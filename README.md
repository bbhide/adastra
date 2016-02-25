# adastra
Automatically exported from code.google.com/p/adastra

Adastra is a BCI application which runs on top of OpenVibe

For the latest updates, please visit the release log.

Currently in trunk: fieldtrip buffer support

General Description:

Adastra is a BCI (Brain Computer Interface) application. Adastra's main goals include:

apply machine learning and DSP in BCI
create an industrial application that is practical, easy to install, has a contemporary look and feel and it is easily extendable
Adastra can work in combination with another BCI application called OpenVibe. Adastra can use real-time data from OpenVibe.

Adastra also supports direct access to Emotiv EPOC (since version 2.1).

Adastra supports executing Octave(Matlab) code thanks to a C# Octave wrapper (since version 2.5).

Adastra is written in Microsoft C# (which is in respect to target goal two).

Please, check Installation and Adastra tutorial.

There are different scenarios supported in Adastra. For example OpenVibe is used to acquire, filter the EEG signal and generate feature vectors from it. Then these feature vectors are forwarded to Adastra's machine learning (ML) algorithms. ML is used to train Adastra to detect actions such as left/right and up/down brain controlled mouse cursor movements.

Several machine learning algorithms are supported: * LDA+MLP: Linear Discriminant Analysis + Multi - Layer Perceptron * LDA+SVM: Linear Discriminant Analysis + Support Vector Machines

Software developers should check the Programming section.

Usage:

it can be used for BCI experiments by researchers
commercial EEG software and/or devices
to help disabled people (to control computers, wheelchairs, etc)
exoskeletons (Iron Man suit?)
Information about future development is provided here.

Related project: Gipsa-lab extensions for the OpenVibe project. Contains a game that you play using EEG amplifier and the P300 paradigm.
