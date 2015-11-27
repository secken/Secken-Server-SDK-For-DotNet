# Secken Private Cloud Server SDK For DotNet

## 简介（Description）
Secken.YangCong.ServerSdk是Secken官方提供了一套用于和洋葱验证服务交互的SDK组件，通过使用它，您可以简化集成Secken服务的流程并降低开发成本。

密码就要大声说出来，开启无密时代，让密码下岗

洋葱是一个基于云和用户生物特征的身份验证服务。网站通过集成洋葱，可以快速实现二维码登录，或在支付、授权等关键业务环节使用指纹、声纹或人脸识别功能，从而彻底抛弃传统的账号密码体系。对个人用户而言，访问集成洋葱服务的网站将无需注册和记住账号密码，直接使用生物特征验证提高了交易安全性，无需担心账号被盗。洋葱还兼容Google验证体系，支持国内外多家网站的登录令牌统一管理。

【联系我们】

官网：https://www.yangcong.com

微信：yangcongAPP

微信群：http://t.cn/RLGDwMJ

QQ群：475510094

微博：http://weibo.com/secken

帮助：https://www.yangcong.com/help

合作：010-64772882 / market@secken.com

支持：support@secken.com

帮助文档：https://www.yangcong.com/help

项目地址：https://github.com/secken/Secken-Server-SDK-For-DotNet

洋葱公有云服务端SDK主要包含三个方法：
* 获取二维码的方法（GetYangAuthQrCode），用于获取二维码内容和实现绑定。
* 请求推送验证的方法（AskYangAuthPush），用于发起对用户的推送验证操作。
* 查询事件结果的方法（CheckYangAuthResult），用于查询二维码登录或者推送验证的结果。
* 复验验证结果的方法（CheckYangAuthToken），用于复验移动端SDK验证的结果。

## 安装使用（Install & Get Started）

To install Secken.YangCong.ServerAuth, run the following command in the Package Manager Console

```
PM> Install-Package Secken.YangCong.ServerSdk
```
## 更新发布（Update & Release Notes）

```
【1.0.1】更新内容：
1、完成了.Net4.5版接口封装。
2、完成了Wp8.0版接口封装。
```

## 要求和配置（Require & Config）
```
// 需要去洋葱开发者中心新建一个类型为SDK的应用，创建完成之后，将对应的AppId+AppKey填过来
private RequestForServerSdkKey _thisRequestServerSdkKey = new RequestForServerSdkKey
{
    AppId = "",
    AppKey = ""
};
/// <summary>
/// ThisRequestServerSdkKey
/// </summary>
public RequestForServerSdkKey ThisRequestServerSdkKey
{
    #region ThisRequestServerSdkKey
    get
    {
        return _thisRequestServerSdkKey;
    }
    set
    {
        if (Equals(_thisRequestServerSdkKey, value)) return;
        _thisRequestServerSdkKey = value;
    }
    #endregion
}
```

## 获取二维码内容并发起验证事件（Get YangAuth QrCode）
```
// 准备请求参数类
var thisRequestServerSdkQrCode = new RequestForServerSdkQrCode(ThisRequestServerSdkKey)
{
    AuthType = MethodForServerSdk.FaceVerify,
    ActionType = "扫码登录", // 可选
    ActionDetail = "举个例子的详情" // 可选
};
// 获取二维码内容的方法
var thisResponseServerSdkQrCode = await ServerSdkProvider.Current.Action<ResponseForServerSdkQrCode>(ServerSdkProviderType.GetYangAuthQrCode, thisRequestServerSdkQrCode);
// 获取二维码内容的结果
if (thisResponseServerSdkQrCode != null)
{
    if (thisResponseServerSdkQrCode.IsLegal)
    {
        // 根据需要返回
    }
} 
```

GetYangAuthQrCode接口包含两个必传参数：ThisRequestServerSdkKey，AuthType；两个可选参数：ActionType、ActionDetail。

|    状态码   | 		状态详情 		  |
|:----------:|:-----------------:|
|  200       |       成功         |
|  400       |       上传参数错误  |
|  403       |       签名错误                |
|  404       |       应用不存在                |
|  407       |       请求超时                |
|  500       |       系统错误                |
|  609       |       ip地址被禁                |

## 查询验证事件的结果（Check YangAuth Result）
```
// 准备请求参数类
var thisRequestServerSdkResult = new RequestForServerSdkResult(ThisRequestServerSdkKey)
{
    EventId = thisRequestEventId
};
// 调用查询事件结果的方法
var thisResponseServerSdkResult = await ServerSdkProvider.Current.Action<ResponseForServerSdkResult>(ServerSdkProviderType.CheckYangAuthResult, thisRequestServerSdkResult);
// 调用查询事件结果的结果
if (thisResponseServerSdkResult != null)
{
    if (thisResponseServerSdkResult.IsLegal)
    {
        // 如果这个UserId和库里面绑定的UserId一致，那就表示可以让他通过
        // 如果这个UserId在库里面查询不到，就可以理解为这是绑定流程。
        if (Equals(thisResponseServerSdkResult.UserId, ""))
        {
            // 根据需要返回
        }
        else
        {
            thisResponseServerSdkResult.Code = ParaForServerSdk.CodeForIllegalForPermission;
            thisResponseServerSdkResult.UserId = null;
            resposeStr = thisResponseServerSdkResult.Serialize();
        }
    }
    else
    {
        thisResponseServerSdkResult.Code = ParaForServerSdk.CodeForIllegalForReturn;
        resposeStr = thisResponseServerSdkResult.Serialize();
    }
}
```
CheckYangAuthResult接口包含两个必传参数，ThisRequestServerSdkKey，RequestEventId。

|    状态码   | 		状态详情 		  |
|:----------:|:-----------------:|
|  200       |       成功         |
|  201       |       事件已被处理                |
|  400       |       上传参数错误  |
|  403       |       签名错误                |
|  404       |       应用不存在                |
|  407       |       请求超时                |
|  500       |       系统错误                |
|  601       |       用户拒绝                |
|  602       |       用户还未操作                |
|  604       |       事件不存在                |
|  606       |       callback已被设置                |
|  609       |       ip地址被禁                |

## 发起推送验证事件（Ask YangAuth Push）
```
// 准备请求参数类
var thisRequestServerSdkPush = new RequestForServerSdkPush(ThisRequestServerSdkKey)
{
    AuthType = MethodForServerSdk.FaceVerify,
    UserId = thisRequestUserId,
    ActionType = "推送", // 可选
    ActionDetail = "举个推送详情的例子" // 可选
};
// 发起推送验证的方法
var thisResponseServerSdkPush = await ServerSdkProvider.Current.Action<ResponseForServerSdkPush>(ServerSdkProviderType.AskYangAuthPush, thisRequestServerSdkPush);
// 发起推送验证的结果
if (thisResponseServerSdkPush != null)
{
    if (thisResponseServerSdkPush.IsLegal)
    {
        // 根据需要返回
    }
}
```
AskYangAuthPush接口包含三个必传参数：ThisRequestServerSdkKey、AuthType、UserId；两个可选参数：ActionType、ActionDetail。  

|    状态码   | 		状态详情 		  |
|:----------:|:-----------------:|
|  200       |       成功         |
|  400       |       上传参数错误  |
|  403       |       签名错误                |
|  404       |       应用不存在                |
|  407       |       请求超时                |
|  500       |       系统错误                |
|  608       |       验证token不存在           |
|  609       |       ip地址被禁                |

## 复验验证结果的方法（Check YangAuth Token）
```
// 准备请求参数类
var thisRequestServerSdkToken = new RequestForServerSdkToken(ThisRequestServerSdkKey)
{
    AuthToken = thisRequestAuthToken
};
// 复验验证结果的方法
var thisResponseServerSdkToken = await ServerSdkProvider.Current.Action<ResponseForServerSdkBase>(ServerSdkProviderType.CheckYangAuthToken, thisRequestServerSdkToken);
// 复验验证结果的结果
if (thisResponseServerSdkToken != null)
{
    if (thisResponseServerSdkToken.IsLegal)
    {
        // 根据需要返回
    }
}
```
CheckYangAuthToken接口包含两个必传参数：ThisRequestServerSdkKey、AuthToken。  

|    状态码   | 		状态详情 		  |
|:----------:|:-----------------:|
|  200       |       成功         |
|  400       |       上传参数错误  |
|  403       |       签名错误                |
|  404       |       应用不存在                |
|  407       |       请求超时                |
|  500       |       系统错误                |
|  608       |       验证token不存在           |
|  609       |       ip地址被禁                |
