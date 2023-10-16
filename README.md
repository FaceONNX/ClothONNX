<p align="center"><img width="30%" src="docs/clothonnx_logo_scaled.png" /></p>
<p align="center"> Cloth segmentation library based on deep neural networks and <b>ONNX</b> runtime </p>  

# ClothONNX
**ClothONNX** is a cloth segmentation library based on deep neural networks and [ONNX](https://onnx.ai/) runtime.

# Version
You can build **ClothONNX** from sources or install to your own project using nuget package manager.
| Assembly | Specification | OS | Platform | Package | Algebra |
|-------------|:-------------:|:-------------:|:--------------:|:--------------:|:--------------:|
| [ClothONNX](netstandard/ClothONNX) | .NET Standard 2.0 | Cross-platform | CPU | [NuGet](https://www.nuget.org/packages/ClothONNX/) | [UMapx](https://github.com/asiryan/UMapx) |
| [ClothONNX.Gpu](netstandard/ClothONNX.Gpu) | .NET Standard 2.0 | Cross-platform | GPU | [NuGet](https://www.nuget.org/packages/ClothONNX.Gpu/) | [UMapx](https://github.com/asiryan/UMapx) |

# Installation
C# interface  
```c#
using ClothONNX;
```
To get started with **ClothONNX**, it is recommended to look at the folder with [examples](netstandard/Examples).  

# License
**ClothONNX** is released under the [MIT](LICENSE) license.
