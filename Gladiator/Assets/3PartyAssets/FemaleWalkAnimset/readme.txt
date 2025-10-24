Female Walk Animset

------------------

This is a set of 39 motion capture animations to build a TPP walking movement for female characters.

------------------
Contents:

 - 39 mocap animations of directional starting, loops, turning and stopping running.
 - a model of a female dummy
 - a MECANIM example graph of how to blend animations

------------------

The MECANIM graph:

The graph is controlled by 7 variables:

1) Horizontal
	Input Horizontal Axis

2) Vertical
	Input Vertical Axis

3) InputMagnitude
	This is a float that should be controlled by Input Axis Vector Magnitude (so for example, how far is the joystick bent). It controls the transitions between Idling and walking.

4) InputDirection
	This is a float that should be controlled by the angle between character's Forward Axis and the Input Axis Vector. So for example, if the joystick is bent right, the angle will be 90 degrees, if back, the angle will be 180 degrees and so on.

5) IsRightLegUp
	This is a bool that should be controlled by which foot is currently higher in a run cycle, so the controller knows which animation it should play while stopping. Easiest way to get this info is checking which foot bone has a bigger Global Transform Y at the moment. So, if right foot is higher - we change IsRightLegUp to true.


6) RotationDirection
	This is a float that should be controlled by the angle between character's Forward Axis and the desired character's rotation. So basically, it controls turning in place. So, when the character is idling - you can control his turning by animation. If the character is walking - you need to control his rotation by SmoothLookAt.

7) LegSwitch
	A float that controls the hips rotation, during walking, to avoid legs crossing at 45 degrees and 135 degrees direction. If the float is 0 - hips are rotated to the right, if 1 - to the left. The controller needs to smoothly change this float, when the legs are apart, and that requires some coding on your side. This is a very advanced system, so if you don't feel up to it, you can safely skip it and have this float always at 0.