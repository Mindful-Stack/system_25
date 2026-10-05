# DiceGameDemo - Moq code along

A Pig-style dice game: roll one dice per round, a 1 wipes your score, anything
else is added to it. The point of the game is that it depends on a dice, a
scoreboard and a notifier - all of which we replace with Moq in the tests.

## Setting the project up during the demo

```bash
dotnet new console -o DiceGameDemo
dotnet new xunit -o DiceGameDemo.Tests
dotnet add DiceGameDemo.Tests reference DiceGameDemo
dotnet add DiceGameDemo.Tests package Moq
dotnet sln add DiceGameDemo DiceGameDemo.Tests
```

Then paste in the files. Run it with `dotnet run --project DiceGameDemo` and
test it with `dotnet test`.

## Production code

| File | What it is |
| --- | --- |
| `Abstractions.cs` | `IDice`, `IPlayer`, `IScoreBoard`, `INotifier` + the exception |
| `Dice.cs` | The real, indeterministic dice. All members `virtual` so it can be mocked as a class |
| `Player.cs` | A real player - simple enough to use as a fake in tests |
| `Infrastructure.cs` | Console notifier + in-memory scoreboard, only used by `Program.cs` |
| `DiceGame.cs` | The system under test |

## Demo 1 - slides 11-14 (`Demo1_TheBasicsTests.cs`)

| Slide | Test |
| --- | --- |
| 11 Create mocks | `CreateAMockAndConfigureIt`, `ALooseMockReturnsDefaultValuesForEverythingWeDidNotConfigure` |
| 11 In context | `PlayRound_AddsTheRollToTheScore`, `PlayRound_WipesTheScoreWhenWeRollAOne` |
| 12 Async | `AsyncMethodsCanBeConfiguredThroughResult`, `AsyncMethodsAreNicerWithReturnsAsync` |
| 12 Lazy returns | `ReturnsCanBeLazyAndChangeBetweenCalls` |
| 12 Throws | `WeCanMakeADependencyThrow` |
| 13 Matchers | `ItIsAny_...`, `ItIsInRange_...`, `ItIsRegex_...`, `ItIs_...` (bonus) |
| 14 Verify | `VerifyThatWeNotifiedThePlayer`, `VerifyThatWeReadAndWroteAProperty`, `VerifyNoOtherCalls_...` |
| → demo 2 | `AMockedPropertyDoesNotRememberWhatWeWriteToIt` - the cliffhanger |

## Demo 2 - slides 16-20 (`Demo2_GoingFurtherTests.cs`)

| Slide | Test |
| --- | --- |
| 16 SetupProperty | `SetupProperty_MakesTheMockRememberWrites`, `SetupAllProperties_...`, `RenamingAPlayerCanNowBeCheckedByState` |
| 17 Strict | `StrictMocksBlowUpOnCallsWeDidNotConfigure`, `StrictMocksPassWhenEveryCallIsConfigured` |
| 17 CallBase | `CallBase_RunsTheRealCodeForWhatWeDidNotMock`, `WithoutCallBase_TheRestOfTheClassIsEmpty` |
| 17 MockRepository | `MockRepository_CreatesAndVerifiesAllMocksTogether` |
| 18 SetupSequence | `SetupSequence_GivesADifferentAnswerEveryTime` |
| 19 MockSequence | `MockSequence_AcceptsCallsInTheExpectedOrder`, `MockSequence_RejectsCallsInTheWrongOrder` |
| 20 Callbacks | `Callbacks_LetUsCountCallsAndCaptureArguments`, `Callbacks_CanCaptureSeveralArgumentsAtOnce` |

## Things worth breaking on purpose

- Delete the `Setup` in `PlayRound_AddsTheRollToTheScore` - the loose mock returns 0, the test fails quietly with no exception. Good lead-in to slide 17.
- Add a second `_dice.Roll()` to `PlayRound` - `VerifyNoOtherCalls_...` and the `Times.Once()` verification both catch it.
- Swap the two lines in `FinishGameAsync` so the save happens first - `MockSequence_AcceptsCallsInTheExpectedOrder` starts failing with the vague "missing setup" message.
- Change `Returns(() => nextRoll)` to `Returns(nextRoll)` in `ReturnsCanBeLazyAndChangeBetweenCalls` - shows that the lambda really is re-evaluated.

## Note on `Moq.Range`

`It.IsInRange(0, 100, Moq.Range.Inclusive)` has to be fully qualified, because
`System.Range` is also in scope through implicit usings.
