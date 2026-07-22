# Introducing dotNetFractal.Uno

This application is built using the Uno Platform. The Uno Platform enables one to write a single codebase in C# and XAML,
which can then be deployed across multiple platforms. My focus is on targeting Windows and macOS.

## Goals

- Multi-threaded fractal generation with a responsive UI, leveraging the Uno Platform for cross-platform compatibility.
- Deep zoom functionality, allowing users to explore fractals in detail.
- Repeatable and consistent rendering of fractal patches with a 'stitcher' to fill large images.

## Purpose

This project serves to get hands-on experience while learning the Uno Platform.
Better yet, the result is a useful application for generating fractal images.
Nowadays, I like to start my working day by generating a fractal image and placing that as my desktop wallpaper.
The application allows me to explore different fractal areas, resolutions, and color maps.
For that reason, the code is hosted on GitHub, at https://github.com/arnoldbono/dotNetFractal.

## Getting Started

- Prerequisites
    * .NET 10 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
    * Uno SDK (https://platform.Uno/docs/articles/get-started.html)
    * Visual Studio 2026 Community Edition (https://visualstudio.microsoft.com/vs/community/), with the following workloads:
        * .NET Desktop Development
        * Universal Windows Platform Development
        * .NET Desktop Development for macOS
    * Alternatively, use Visual Studio Code (https://code.visualstudio.com/)
- Build/run commands
    * dotnet restore
    * dotnet build
    * dotnet run (from the dotNetFractal.Uno project directory)

## Original Codebase

In 1992, I bought the book "Fractal Programming in C" by Roger T. Stevens. The first code I probably wrote was in Turbo Pascal 3.0.
When in University, I wrote a fractal generator in C on a HP 9000/720 to teach myself how to write code on Unix.
The values were all hard-coded and the program probably was a command-line application.

When learning C# and WinForms, I wanted to beautify the fractal image by using the fraction of two colors; one from the last iteration that was inside the threshold circle radius
and the second from the next iteration that stepped outside of it. Lack of design skill and time resulted in a scrappy-looking application.

To teach myself WPF and MVVM, I did a one-to-one port of the WinForms application to WPF. Lack of time, focus, and attention caused the application to be a bit of a mess.
One idea that I had was to make the fractal generation multi-threaded, by stitching 128 x 128 pixel patches. It sort of worked, but the main thread was still overloaded, resulting in a non-responsive UI.
Another idea was to make patches like on Google Maps, where the user could zoom in and out and the application would serve the patches on demand. That part never got off the ground.

## New WPF Codebase

With the help of GitHub Copilot in June 2026, I started making the application more functional. Once everything was working using dialogs, the Properties panel was implemented.
The Properties panel was a good step forward in design and usability, as it allowed the user to change the fractal area, resolution, and color map without having to open and close modal dialogs.

The zoom level that can be achieved with the floating-point representation <code>double</code> is limited.
The solution is to use <code>decimal</code> instead of <code>double</code>, which has a much higher precision.
The option "High Precision" was added to the Properties panel, which allows the user to switch from <code>double</code> to <code>decimal</code>.

To improve performance, the fractal computation assumes that when all pixels on the edge of a *n* x *n* patch reach max iteration, that the entire patch is likely to be 'inside the fractal set'.
If only some of the edge pixels reach max iteration, the patch is subdivided into 4 patches of *n/2* x *n/2*. It leads to a serious performance improvement, as the fractal computation is O(n^2)
and the patch subdivision is O(log(n)). The patch subdivision is recursive, so it can be subdivided multiple times. The subdivision stops at 16 x 16 since there is also overhead of stopping and starting threads.

Another improvement is bit-blitting fractal patches to a bitmap, which is then displayed in the UI. This is much faster than setting each pixel individually.

The final improvement, still in development, is a Distribution Graph, which shows the distribution of pixel values in the fractal image.
It is useful for understanding how the color map will affect the final image.

## New Uno Codebase

Mid July 2026, I started porting the WPF codebase to the Uno Platform, again with the help of GitHub Copilot.
The result was a duplication of the WPF codebase; *dotNetFractal.WPF* now had a counterpart called *dotNetFractal.Uno*.
To reduce duplication, I created a new project called *dotNetFractal.UI*, which contains all the shared code (ViewModels, Models, and Converters).
The WPF and Uno projects reference the UI project. The class <code>SharedMainViewModel</code>, for example has two derived implementations, one for WPF and one for Uno.
The platform-specific implementations inject the correct <code>IBitmapConverter</code> implementation, for example.
Dependency Injection could be used to avoid the need for the derived classes, but I wanted to keep the code simple and easy to understand.
Also, it helps to have a parameterless constructor for the ViewModel, which is useful for XAML design-time support.

## Current Status

- It is possible to generate fractal images in both the WPF and Uno applications.
- The Properties panel is functional, but the Distribution Graph is not yet implemented.
- The Forward and Backward buttons and hotkeys are not yet implemented.
- The hotkeys for Maximize image (F11) and Zoom to fit (F10) are not yet implemented.
- Upon zooming in, the selected area is the basis of the blown-up bitmap shown at the start of the new fractal computation. It is incorrect at deeper zoom levels.

## Intermediate Tasks

- Clean up the Uno project, to get the quality level up on par with the WPF project.

## Future Improvements

- The color map editor could be improved by allowing non-linear interpolation between colors.
- Use <code>BigDecimal</code>, for arbitrary precision.
