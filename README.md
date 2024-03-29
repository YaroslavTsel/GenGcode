# Gencode

## Tutorial video
[![GenGcode](https://img.youtube.com/vi/vxhWgF7xYfM/0.jpg)](https://www.youtube.com/watch?v=UV89FhFmKaY)


## Disclaimer
I have a laser cutter/engraver, and one of the most annoying thing was all these file conversions: from dwg to pdf (OMG!), from pdf to svg, from svg to gcode.
Ofcourse, I know about free online converters, but they work unstable. So, I decided to write my own Autocad plugin to do it easy and fast.
## How to use
At first prepare your drawing in Autocad. Convert everything to lines, arcs, polylines and circles, place it on different layers for cut and engrave. You can define up to 8 layers.


<img src="https://user-images.githubusercontent.com/119655739/205248321-e6b5cce8-4112-4670-8d73-c599d305eede.png" alt="drawing" width="350"/>

Then type "NETLOAD" in Autocad and load the "GenGcode.dll". You can see mini instruction on pop-up window. 

<img src="https://user-images.githubusercontent.com/119655739/205249079-a4cd079f-0f1a-41b2-82f8-70a30db430b1.png" alt="drawing" width="350"/>

If you want to get gcode for only a few items, you can select them now in Autocad.After that type "GETGCODE". The settings window will appear so you can setup how do you want engrave each layer. All settings will be saved in the drawing and you can change them any time by typing "SETGCODE". 

<img src="https://user-images.githubusercontent.com/119655739/221532215-10bdc69e-1c68-458c-b849-a6a78493db54.png" alt="drawing" width="350"/>

Here you can check the "pass by element" option. So, laser will do all passes for one element and then goas to the next one. If it is not checked, laser will pass all elements on layer every time. Dynamic power mode will automatically adjust laser power based on the current speed relative to the programmed rate. To use it, your laser must support "M4" command.

As a result you'll get gcode file wich you can use with engraving programs or upload directly to your engraving laser. If you have selected several elements, the suffix "_Selected" will be added to the file name, otherwise the suffix "_All". The first command to the laser is walk all perimeter. It's just to ensure that there is enough space. That is all! 

UPD:
Before using downloaded "GenGcode.dll" you have to unlock it in its properties:

<img src="https://user-images.githubusercontent.com/119655739/210131889-719f2b06-f319-4217-bc75-875ff35d7d7f.png" alt="drawing" width="350"/>


