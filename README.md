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

## Time reversal

Furier transform of signal is the important parameter of using fit. Firstly, it creates a tensor of **u**(**x**, variety) function values, after that it creates a tensor of **u**(**x**, **t**) function values and prepare this tensor according to selected *metric*. After all u can see *the surface* to determine the location of defect.

There are several important hyper-parameters for determining defect's location: sensors' location and variety, time and space nets (the thicker net is better, but takes more time for calculating).

### Example: piezoelements choosing and start calculations


### Example: see results


## Ellipse method


## Features

