# SlicerUtil

This repository contains some of utility files of 3D-printer slicer - polygon optimizer, some of settings, GUI and tests for polygon optimizer and settings.

# PolygonOptimizer
PolygonOptimizer is used to calculate optimal sequence of printing paths to achieve minimum printing time and best quality. Previous module has produced data as List<List<Segment>> where List<Segment> is a polygon. 
PolygonOptimizer implements several algorithms to calculate sequence:
- by shortest distance
- by best point

PolygonOptimizer provides methods to calculates the best point. The best point for 3D-printing is the point on the polygon located in the sharpest concave cavity. Starting ans stopping the printing in that point provides good quality and allows to not have a plastic excessives.

<h2>Example</h2>
<h3>Input data</h3>

![image](http://support.anisoprint.com/wp-content/uploads/img/op1.png)

<h3>By best point</h3>

![image](http://support.anisoprint.com/wp-content/uploads/img/op3.png)

<h3>By closest point</h3>

![image](http://support.anisoprint.com/wp-content/uploads/img/op2.png)

# Settings

The Slicer provides more than 250 settings to maintain printing process. They are devided to Material, Profile, Printer and Session modules. This repository contains Material, Printer, Session and Profile settings wrapper.
ISettingsMemento is the interface which allows to correctly manipulated with any settings (show in GUI, executes file operations) without knowing axact type of settings.
SettingsStore provides storage and add/remove any settings.
SessionStore provides methods to organize correct storage of concurent settings. Concurency depends on version of settings, default/user type and etc.

#GUI

The GUI developed with WPF framework. It uses MVVM-pattern and maintain with Stores and Settings (as Model in MVVM). The GUI also uses proprietary devDept.Eyeshot 3D-engine to works with 3D-graphics.

<h3>Main view</h3>

![image](http://support.anisoprint.com/wp-content/uploads/img/Screenshot_1.png)

<h3>Geometry view</h3>

![image](http://support.anisoprint.com/wp-content/uploads/img/Screenshot_2.png)
