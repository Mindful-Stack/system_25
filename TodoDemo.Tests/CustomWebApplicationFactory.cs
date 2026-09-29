using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TodoDemo.Data;

namespace TodoDemo.Tests;

// En "fabrik" som startar hela vår webbapp inuti testprocessen.
// Allt är precis som i den riktiga appen, utom en sak: databasen.
// Den byter vi mot en tillfällig SQLite-databas som bara finns i minnet.
public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
{
    // ConfigureWebHost ger oss en chans att ändra i appen innan den startar.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // "services" är appens tjänstecontainer (dependency injection).
        // Tänk på den som en uppslagstabell:
        //     typ (till exempel TodoDbContext)  ->  recept för hur man skapar en sådan
        // När något i appen behöver en tjänst frågar det containern efter TYPEN,
        // och containern följer receptet som står på den raden.
        //
        // Koden här inne körs EFTER att Program.cs har fyllt i sina rader.
        // Därför måste vi först ta bort appens rader och sen lägga in våra egna.
        builder.ConfigureServices(services =>
            {
                // 🧹 Steg 1: Ta bort appens recept för den riktiga databasen.
                // Varje rad i uppslagstabellen kallas en "descriptor".
                // AddDbContext i Program.cs lade in två rader som handlar om databasen:
                //   - DbContextOptions<TodoDbContext>: de färdiga inställningarna
                //   - IDbContextOptionsConfiguration<TodoDbContext>: koden som skriver
                //     inställningarna, alltså "använd todos.db"
                // Vi letar upp båda och tar bort dem. Glömmer vi den andra körs appens
                // databasval ändå, och om appen använder SQL Server krockar det med vår SQLite.
                
                // Find db service
                var descriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
                // Remove it
                if (descriptor != null)
                    services.Remove(descriptor);

                // Same for db config
                var configDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<TodoDbContext>));
                if (configDescriptor != null)
                    services.Remove(configDescriptor);

                // 🧹 Steg 2: Ta bort ett eventuellt gammalt recept för SqliteConnection.
                // Vår app registrerar ingen sådan, men om den gjorde det skulle tabellen ha
                // två rader för samma typ, och containern lämnar alltid ut den som lades in sist.
                // Genom att städa bort den gamla vet vi att raden i steg 3 blir den enda.
                
                // Same for db connection
                var dbConnectionDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(SqliteConnection));
                if (dbConnectionDescriptor != null)
                    services.Remove(dbConnectionDescriptor);

                // 🧪 Steg 3: Lägg in ETT recept för en SQLite-anslutning i minnet.
                //
                // Två saker att veta om SQLite i minnet:
                //   - databasen lever bara så länge anslutningen till den är öppen
                //   - varje NY anslutning till ":memory:" får en helt ny, tom databas
                // Alltså: en öppen anslutning = en databas.
                //
                // AddSingleton betyder: skapa EN enda anslutning och lämna ut samma
                // anslutning varje gång någon frågar efter typen SqliteConnection.
                // Koden inuti körs inte nu, utan första gången någon frågar.
                // Sedan sparas anslutningen och återanvänds under hela testkörningen.
                services.AddSingleton<SqliteConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    return connection;
                });

                // 🧪 Steg 4: Lägg in ett nytt recept för TodoDbContext.
                //
                // Appen skapar en ny TodoDbContext för varje HTTP-anrop.
                // Varje gång det händer körs koden nedan, och den frågar containern
                // efter typen SqliteConnection. Den vet ingenting om steg 3, den frågar
                // bara efter typen. Men steg 3 är den enda raden för den typen,
                // så svaret blir alltid samma öppna anslutning.
                // Resultat: alla anrop i ett test pratar med samma databas i minnet.
                services.AddDbContext<TodoDbContext>((container, options) =>
                {
                    var connection = container.GetService<SqliteConnection>();
                    options.UseSqlite(connection);
                });
            }
        );
    }
    
    // CreateHost körs när appen byggs. Vi skriver över den för att göra en sak till
    // innan första testet körs: skapa tabellerna i den tomma databasen i minnet.
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder); // Bygg appen som vanligt: Program.cs plus våra ändringar ovan.

        // Utanför ett HTTP-anrop måste vi själva öppna ett "scope"
        // för att få låna en TodoDbContext av containern.
        // "using" gör att scopet, och contexten i det, städas bort direkt efteråt.
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        // Det här är första gången någon ber om en TodoDbContext. Då körs receptet i steg 4,
        // som frågar efter anslutningen, och då körs receptet i steg 3: anslutningen skapas och öppnas.
        // EnsureCreated skapar sedan tabellerna utifrån vår modell (TodoItem).
        db.Database.EnsureCreated(); 

        return host; // Lämna över den färdiga appen till testerna.
    }
}