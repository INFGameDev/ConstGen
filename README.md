# Const Generator 
Unity Constants Generator 

![ConstGen Window](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/ConstGenWIndow.PNG)

Tested on Unity Versions: <br/>
[ 2019.4.15f ] <br/> [ 2018.4.14f ]

#### Const Generator generates constant properties within static classes replacing the usage of magic strings by holding the value of those strings in the given constant property.
.<br/>
.<br/>
.<br/>
.<br/>
.<br/>
but why the use for this, you ask?<br/>
why not just use magic strings and simply write it like this:<br/>

 `animator.SetFloat("XMove", 5);`ノ( ゜-゜ノ)

Well....<br/>
Because screw this! (╯°□°）╯︵ `;(ϛ ',,ǝʌoWX,,)ʇɐolℲʇǝS˙ɹoʇɐɯᴉuɐ`

<br/>

Magic strings are considered bad as they are very error-prone, they are non-performant and slow as no optimization is applied on them by the compiler as they are dynamically changing. The more you use them the more it will add up that will not only cause performance problems but will also make it harder to fix the bugs on your code.

So instead so we use constants, by this way we are fixing a variable's value so that it can be optimized, not only that it also eliminates the error of writing a typo in which the compiler has no whatsoever idea but will instead propagate an error at runtime causing annoying problems and bugs. 

In here instead of typing out the string directly as an argument to a method, the constant property is used.

![usage example](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/Usage_Example.png)

By this way you are sure there is no typos on the string you are passing. <br/>
Plus you got some of that neat pop up suggestions on other related constant properties and auto-completion :ok_hand:

- - - -

## Features ##

### ( Constants Generation ) ###
#### ConstGen can generate the type of unity constants for: ####
- [x] Layers
- [x] Tags
- [x] Sorting Layers
- [x] Scenes
- [x] Nav Mesh Areas
- [x] Input Axes
- [x] Shader Properties
- [x] Animator Controller Parameters
- [x] Animator Controller Layers
- [x] Animator Controller States <br/>

### ( Custom Generators ) ###
#### ConstGen can also create custom generator for generating constants ####
![generator creation](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/GeneratorCreation.png)

### ( Generating Enums ) ###
![Generated Enum](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/generated_enum.png)

- - - -

## Usage ##

Access the ConstGen window in the Main Menu <br/>
![accessing the window](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/Main_Menu.png)

### ( Generating Constants ) ###
![generating constants](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/GeneratingConstants.png)

### | Settings | ###

**[Close On Generate]** - You can treat the window like a pop up that when everytime you generate, the window will close automatically or leave this option off and you can dock the window and generate files without it closing.
 
![indentifier](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/IdentifierFormat.png) <br/>
**[Identifier Format]** - Defines how the the class names of the generated constants are formatted.
- Under_Score_Divider
  - Filters the whitespaces and invalid charaters of the class names into underscores.
- Pascal Case No Divider
  - Removes the whitespaces and invalid charaters of the class and merge into a pascal case naming format.

**[ReGen On Missing]** - Sets the generator to generate it's constants file if it detected none exists. <br/>

NOTE: [Force Generate] button depends on this setting as it will delete the constants file and let the generator create a new one.

**[Update On Reload]** - Sets the generator to automatically generate/update it's constants file on editor recompile if any changes is detected within the unity editor, e.g adding new layers or deleting animator controller paramters. <br/>

NOTE: All generator update checks are are done upon editor recompile so the generator won't trigger script generate and recompile every after little change you want on the editor constants. 

### | Generation | ###

**[Generate]** - Updates the constants or generates the constants file of the generator if none is present.

**[Force Generate]** - Deletes the constants file and let the generator regenerate a new one.

**[Generate ALL] & [Force Generate ALL]** - you know.....just like the generate & force generate buttons but instead triggers all generators.

### | Constants Files/Generated Files | ###
![generated files](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/generated_files.PNG)

### Generation Output Directory Paths ###

**Editor Constants** => Scripts/ConstGen Files/Generated Constants <br/>
**Custom Generators** => Scripts/ConstGen Files/Custom Generators <br/>
**Enums** (Default) => Scripts/ConstGen Files/Generated Enums <br/>

NOTE: DO NOT move around the files inside the ConstGen folder as it will break how the generators look for the files it needed inside ConstGen Folder BUT you can move the ConstGen folder itself at any directory in the assets. <br/>

ANOTHER NOTE: Also DON'T change or move the folder of **Generated Constants** cause the generators look for that specific directory for the files inside when updating and generating files.

AND ALSO ANOTHER ONE: In the event of for some reason the generated files has an error and [Force Generate] won't delete the file, you can manually delete the file itself in the it's folder with the **[ReGen On Missing]** turned on and the generator/s will try to generate a new file.


### ( Creating Custom Generators ) ###
![Creating generators](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/generator%20creation2.PNG)

You can also create custom generator scripts like the ones ConstGen use to generate the constants properties through script. <br/>
This this will generate a custom generator template that you can modify and get started on.

**Generated Name** - Already self explanatory, this will also be the name of the generator script. <br/>
**Output File Name** - The name of the generated file by the generator which is also the generated file's script name. <br/>
**Output Type** - The data type that the generated constants will store. <br/>
**Output Path** - The directory in which the generate constant/file is created at. 

- - - -

## Using The Constants ##
![how to use](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/namespaceImport.PNG)

Import the `ConstGenConstants` namespace on which the constants are in and from there you can access them.

- - - -

## Generating Enum Constants ##
![Generating Enums](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/GeneratingEnums.PNG)

`List<EnumConstant>` - This is the list where you will add all the enums you want to generate. <br/>
`EnumConstant` - The enum data type that will hold all the constants you'll be generating, you pass the name of the enum to the object constructor at instance initialization. <br/>
`enum_.Add("Forward", 0 );` - adds an constant into the enum, passing it's name and int value. <br/>

NOTE: if you don't want to assign a value to it, you can pass in `null` and none will be assigned thus the enum will instead have it's index as it's value.

then you add the `EnumConstant` variable into the `List<EnumConstant>` list. you can more than one enum in the list and the generator will generate those into the file.

`ConstGen.ConstantGen.GenerateEnums("Directions", enumList);` - Generates the enums into the default enum ouput directory with the file name of "Directions".

NOTE: you shorten the method call by importing the `ConstGen` namespace and there are other `GenerateEnums` override methods you can utilize. 

- - - -

![meme](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/no%20magic%20strings%20meme.png)

- - - -
The code generation process is based from srndpty's CodeGen: https://github.com/srndpty/CodeGen
