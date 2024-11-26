## 说明 
本项目为哔哩哔哩开放平台`C#`版Demo案例，用于验证签名以及各项接口基础能力的示例。  

本SDK实现为基础功能，更多高级功能以及使用请自行探索，以及根据具体的业务功能和需求自行修改。

开放平台相关文档请访问：[哔哩哔哩开放平台文档中心](https://open.bilibili.com/doc)

## 环境要求
`Visual Studio 2022 + dotnet8`

## 使用范围
本签名示例的覆盖范围为[哔哩哔哩开放平台文档中心](https://open.bilibili.com/doc)中相关接口的签名实现，不包含[直播创作者服务中心](https://open-live.bilibili.com/document/bdb1a8e5-a675-5bfe-41a9-7a7163f75dbf#h1-u5E73u53F0u4ECBu7ECD)中的相关接口，请注意。

## 使用方法
在`OpenPlatformSample`项目的`Program.cs`文件中`Init()`函数中的几个值需要填写的内容中填写注释说明中对应的内容。
```C#
        /// <summary>
        /// 初始化，启动时必须最先调用，配置应用基础信息，使用前请替换成自己的应用信息
        /// </summary>
        public static void Init()
        {
            OpenPlatform_Signature.Signature.Client_ID = Secrest["Client_ID"];//入驻开放平台后，通过并且创建应用完成后，应用的Client_ID（https://open.bilibili.com/company-core）
            OpenPlatform_Signature.Signature.App_Secret = Secrest["App_Secret"];//入驻开放平台后，通过并且创建应用完成后，应用的App_Secret(https://open.bilibili.com/company-core)
            OpenPlatform_Signature.Signature.ReturnUrl = Secrest["ReturnUrl"];//创建应用后，开发者自行设置的'应用回调域'（https://open.bilibili.com/company-core/{Client_ID}/detail）
        }
```
然后根据需要，使用Sample中Main里的各项功能，**请注意，请根据自己的需求选择对应的函数启动，默认情况下，所有函数都在Main中，可能会造成冲突以及流程过长**
