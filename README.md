# cs-console-progress
Simple C# console progress bar for reuse in console applications.

![Sample progress bar](/progressBarSample.png?raw=true "Sample progress bar")

## ConsoleProgressBar ##

##### ConsoleProgressBar.#ctor(UInt32, Int32, UInt32, ConsoleColor, ConsoleColor)

 Constructor. 

|Name | Description |
|-----|------|
|totalUnitsOfWork: |Total amount of work. Used for calculating current percentage complete.|
|startingPosition: |Progress bar starting position. Defaults to 0.|
|widthInCharacters: |Size of progress bar in characters. Defaults to 40.|
|completedColor: |Color for completed portion of progress bar. Defaults to Cyan.|
|remainingColor: |Color for incomplete portion of progress bar. Defaults to Black.|


---

##### ConsoleProgressBar.Draw(UInt32)

 Draws progress bar. 

|Name | Description |
|-----|------|
|currentUnitOfWork: |Current unit of work in relation to TotalUnitsOfWork.|

---
