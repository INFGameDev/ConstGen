
# Change Log
All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).
 
## [1.4.1] - 2020-02-25
 
### Added
- Input Axis Constant Generation
- Enums generation through script
- Close on generate toggle
- Constant name indentifier format type option
- Custom generator output data type option
- File output directory path on custom generator option
- CustomGenBase for generating custom generators
- New Custom Generator Template File

### Changed
- All generated files are created outside the ConstGen asset folder
- The Generator for creating Custom Generators now inherits from the generator base class of it's own
- Core scripts is moved outside of the Editor folder so generators that is designed to used by non
  Editor scripts can access it to generate files
- Generate All and Force Generate All buttons are group together with the constants generation buttons
- Forge Generate now deletes the folder of the generated constants instead of deleting the files one by one

### Removed
- Old versions of Custom generator script and it's old template txt file
 
## [1.0.1] - 2020-02-20
  
### Fixed
- Fixed scene constant generator where it outputs the filepath of the scene as the constant name
  instead the name of the scene.
 
## [1.0.0] - 2020-02-20
   
### Added
- All source files.
- License
- Readme
- Changelog

### Changed
- Separated the directory of generated files and created generators from the asset's folder