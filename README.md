# Search for defects in plates

Application for searching for defects in plates (Ellipse method and Time-reversal)

The project has many dependences with [math classes library](https://github.com/PasaOpasen/MathClasses)

## What should you do to run examples?

1. Install **R** > 3.6 (u can use [this site](https://cran.r-project.org/bin/windows/base/))

2. Set **Rscript.exe** as default application to running .r files

3. Run [this file](https://github.com/PasaOpasen/Search-for-defects-in-plates/blob/master/Defect2019/Resources/InstallPackages.R) (as administator) to install necessary packages

## Signal preparation

### Algorithm:

1. Record "clean" signal (just signal from default material)

2. Record signal after creating defect

3. Create the difference between these signals

4. Create Furier transform of this difference

### Example: creating difference

![1](https://github.com/PasaOpasen/Search-for-defects-in-plates/blob/master/gifs/create_diff.gif)

### Example: viewing difference and it's Furier transform

![1](https://github.com/PasaOpasen/Search-for-defects-in-plates/blob/master/gifs/trans.gif)

## Piezoelements choosing

Furier transform of signal is the important parameter of using fit. Firstly, it creates the tensor of $**u**(**x**, variety)$ function values

## Time reversal


## Ellipse method


## Features

