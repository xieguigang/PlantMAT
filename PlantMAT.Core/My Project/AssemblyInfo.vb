Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
#if netcore5=0 then 
' 有关程序集的一般信息由以下
' 控制。更改这些特性值可修改
' 与程序集关联的信息。

'查看程序集特性的值

<Assembly: AssemblyTitle("Plant Metabolite Annotation Toolbox")>
<Assembly: AssemblyDescription("PlantMAT: A Metabolomics Tool for Predicting the Specialized Metabolic Potential of a System and for Large-Scale Metabolite Identifications")>
<Assembly: AssemblyCompany("PlantMAT")>
<Assembly: AssemblyProduct("PlantMAT.Core")>
<Assembly: AssemblyCopyright("Copyright © PlantMAT 2020")>
<Assembly: AssemblyTrademark("PlantMAT")>

<Assembly: ComVisible(False)>

'如果此项目向 COM 公开，则下列 GUID 用于 typelib 的 ID
<Assembly: Guid("2a2a432d-3269-4577-b590-ebcbc6ad00cc")>

' 程序集的版本信息由下列四个值组成: 
'
'      主版本
'      次版本
'      生成号
'      修订号
'
'可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值
'通过使用 "*"，如下所示:
' <Assembly: AssemblyVersion("1.0.*")>

<Assembly: AssemblyVersion("2.145.*")>
<Assembly: AssemblyFileVersion("1.33.*")>
#end if