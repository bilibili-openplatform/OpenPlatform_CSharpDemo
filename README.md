## 说明 
本项目为哔哩哔哩开放平台`C#`版Demo案例，用于验证签名以及各项接口基础能力的示例。  

本SDK实现为基础功能，更多高级功能以及使用请自行探索，以及根据具体的业务功能和需求自行修改。

开放平台相关文档请访问：[哔哩哔哩开放平台文档中心](https://open.bilibili.com/doc)

## 注意事项
该demo中提到的所有功能，都需要通过开放平台接入后，且相关用户授权完成后才能正确触发

## 功能完成情况
目前该Demo还未完全完成，已完成部分Demo，已完成内容如下图  
例：  
✅已完成|❌开发中|➖需要第三方服务回调或配合

|模块|完成状况|
|--|--|
|签名算法|✅|
|网页应用接入|✅|
|账号授权|✅|
|换取Token|✅|
|获取直播间基础信息|✅|
|获取直播间长连及场次心跳ID|✅|
|直播间长连心跳|✅|
|直播间长连接批量心跳|✅|
|获取开播授权连接|✅|
|开播授权回调|➖|
|查询用户已授权权限列表|✅|
|获取用户授权公开基础信息|✅|
|服务端视频稿件文件上传预处理|✅|
|视频稿件文件分片上传|✅|
|视频稿件文件分片合片|✅|
|视频稿件单小文件上传|✅|
|视频稿件封面上传|✅|
|视频稿件提交|✅|
|唤起粉端投稿|➖|
|单一视频稿件详情查询|✅|
|视频稿件详情列表查询|✅|
|视频稿件编辑|❌|
|视频稿件删除|❌|
|专栏文章提交|❌|
|专栏文章编辑|❌|
|专栏文章删除|❌|
|查看专栏文章详情|❌|
|查看专栏文章列表|❌|
|获取专栏文章分类列表|❌|
|获取视频、文章卡片信息|❌|
|文集提交(创建)|❌|
|文集信息编辑|❌|
|文集下文章列表修改|❌|
|文集删除|❌|
|查询文集列表|❌|
|查询文集详情|❌|
|专栏文章图片上传|❌|
|获取用户数据|❌|
|获取单个视频数据|❌|
|获取用户整体稿件近30天增量数据|❌|
|获取单个专栏数据|❌|
|获取用户整体专栏近30天增量数据|❌|
|平台活动任务接入|需第三方定制对接|
|交易电商|电商相关的几十个接口需第三方定制对接|

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

## 快速复用鉴权内容
- 本Demo项目中使用了VS2022总`用户机密`进行敏感信息管理，如需快捷复用基础信息，可以在机密信息中填写内容方便使用。
![f8cbf27278a31d9d76b63a67f176e43e](https://github.com/user-attachments/assets/845f772a-8dc2-4c01-8d61-0d6e5b785f2a)


- secrets.json示例内容：
```json
{
  "AccessToken": "",//用户授权后使用code兑换的token信息
  "OpenId": "",//使用token查询到的该用户的open_id
  "Client_ID": "",//应用的Client_ID
  "App_Secret": "",//应用的App_Secret
  "ReturnUrl": "",//应用配置的'应用回调域'
  "resource_id": ""//方便用于查询，对应open_id用户名下的稿件BV号
}
```

