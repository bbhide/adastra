******************************************************
* Thanks for using the OpenViBE Skeleton Generator ! *
******************************************************

File generation completed
[Tue Feb 21 08:38:06 2012]
-------------------------

WARNING:
Writing a new driver for an EEG device is hardly dependent on hardware specifications, such as OS compatibility
or communication protocol. The generator gives you only the basic skeletons for easy integration with OpenViBE, 
please refer to your device specifications (API, protocol description, etc.) for the next steps...

The generator produced the following files:

The Driver class:
- ovasCDriverTestDriver.h
- ovasCDriverTestDriver.cpp
You can put the driver files in the trunk directory in your local repository:
     [openvibe-repository]/trunk/openvibe-applications/acquisition-server/trunk/src/[my-device]/

The configuration class:
- ovasCConfigurationTestDriver.h
- ovasCConfigurationTestDriver.cpp
You can put these files in the trunk directory in your local repository:
     [openvibe-repository]/trunk/openvibe-applications/acquisition-server/trunk/src/[my-device]/

The glade interface:
- interface-TestDriver.glade
You can put this file in the trunk directory in your local repository:
     [openvibe-repository]/trunk/openvibe-applications/acquisition-server/trunk/share/openvibe-applications/acquisition-server/

Don't forget to declare your driver in the Acquisition Server application.
Look in ovasCAquisitionServerGUI.cpp, you will find examples of such declarations.

For more information about building a new driver and filling your skeleton, please read the official tutorial:
http://openvibe.inria.fr/tutorial-creating-a-new-driver-for-the-acquisition-server/

Feel free to propose your contribution on the forum ! 
http://openvibe.inria.fr/forum/

Enjoy OpenViBE !

- The development team -
