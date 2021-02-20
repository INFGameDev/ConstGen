- - - -

#### If you like this asset, a donation of any value is very much appreciated [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=RTBZPSYEFNUGG)

- - - -

# Const Generator
Unity Constants Generator

![ConstGen Window](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/ConstGenWIndow.PNG)

Tested on Unity Versions: <br/>
[ 2019.4.15f ] <br/> [ 2018.4.14f ]

#### Const Generator generates constant properties within static classes replacing the usage of magic strings by holding the value of those strings in the given constant property. ####
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

Magic strings are considered bad as they are very error-prone, they are non-performant and slow as no optimization is applied on them by the compiler as they are dynamically changing, the more you use them the more it will add up that will not only cause performance problems but will also make it harder to fix the bugs on your code.

So instead so we use constants, by this way we are fixing a variable's value so that it can be optimized, not only that it also eliminates the error of writing a typo in which the compiler has no whatsoever idea but will instead propagate an error at runtime causing annoying problems and bugs. 

In here instead of typing out the string directly as an argument to a method, the constant property can be used. 

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
- [x] Shader Properties
- [x] Animator Controller Parameters
- [x] Animator Controller Layers
- [x] Animator Controller States
- [x] Nav Mesh Areas <br/>

### ( Constant Generator Creation ) ###
#### ConstGen can also create generator scripts ####
![generator creation](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/GeneratorCreation.png)

- - - -

## Usage ##

### ( Generating Constants ) ###
![generating constants](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/GeneratingConstants.png)

### Settings ###

**[ReGen On Missing]** - Sets the generator to generate it's constants file if it detected none exists.

NOTE: the [Force Generate] button depends on this setting as it will delete the constants file and let the generator create a new one.

**[Update On Reload]** - Sets the generator to automatically generate/update it's constants file on editor recompile if any changes is detected within the unity editor, e.g adding new layers or deleting animator controller paramters.

NOTE: All generator update checks are are done upon editor recompile so the generator won't trigger script generate and recompile every after little change you want on the editor constants. 

### Generation ###

**[Generate]** - Updates the type of constants or generates the file is none is present.

**[Force Generate]** - Deletes the file on the type of constants and let the generator regenerate a new one. 

**[Generate ALL] & [Force Generate ALL]** - you know.....just like the generate & force generate buttons but instead triggers all generators.

### Constants Files/Generated Files ###
![generated files](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/generated_files.PNG)

Constants files are generated at (ConstGen/Generated Files) directory.

NOTE: Don't move around the files inside the ConstGen folder as it will break the generators but you can move the ConstGen folder itself at any directory in the Assets.

ANOTHER NOTE: In the event of for some reason the generated files has an error and [Force Generate] won't delete the file, you can manually delete the file itself in the it's folder with the **[ReGen On Missing]** turned on and the generator/s will try to generate a new file.


### ( Creating Generators ) ###
![generating generators](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/generator%20creation2.PNG)

You can create generator scripts like the ones ConstGen use to generate the constants properties you want through script. 

**Generated Name** - Already self explanatory, this will also be the name of the generator script.

**Output File Name** - The name of the generated file by the generator which is also the generated file's script name.

Created generators are generated in (ConstGen/Editor/Generated Generators)

- - - -

## Using The Constants ##
![how to use](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/namespaceImport.PNG)

Import the `ConstGenConstants` namespace on which the constants are in and from there you can access them.

- - - -

![meme](https://github.com/INFGameDev/Project-ReadMe-Images/blob/master/ConstGen/no%20magic%20strings%20meme.png)

- - - -

The code generation process is based from srndpty's CodeGen: https://github.com/srndpty/CodeGen
