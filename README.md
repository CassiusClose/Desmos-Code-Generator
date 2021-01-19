<h1>Desmos Code Generator</h1>

<h4>A tool that generates Desmos Computation Layer code that displays error-checking messages for a multiple choice question.</h4>
<h5>Cassius Close</h5>

![Program Screenshot](../examples/ProgramScreenshot.png)

<h3>The Application</h3>
Desmos Computation Layer is a language that provides customizable functionality of components within Desmos activities.
One such component is a Multiple Choice Question. My mother is a math teacher and wanted to display a message to her students when they answered a multiple choice question that would either tell them
that they were correct or describe in what way they were not.
When you have multiple choice questions that have many choices and several correct ones, it becomes painstaking to manually cover every possible combination of selections with Computation Layer, so
I wrote a program that would generate the code to do this.

<br>
The user is able to pick the number of options and customize the messages for each case, depending on how many correct and incorrect choices the user selected.

<h3>Contents</h3>

This is a Windows application written using WPF within Visual Studio. The program is contained in DesmosCodeGen/ and the publication project is contained in CodeGenWiXSetup/.
 
