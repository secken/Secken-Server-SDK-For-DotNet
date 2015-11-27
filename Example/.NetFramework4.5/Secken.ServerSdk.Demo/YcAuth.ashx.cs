using System;
using System.Web;
using Secken.ServerSdk.Framework;
using Secken.ServerSdk.Models;
using Secken.ServerSdk.Models.Request;
using Secken.ServerSdk.Models.Response;

namespace Secken.ServerSdk.Demo
{
    /// <summary>
    /// YcAuth 的摘要说明
    /// </summary>
    public class YcAuth : HttpTaskAsyncHandler
    {
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

        /// <summary>
        /// 这个根据自己业务来，Demo中用它来做登录的Cookie Token
        /// </summary>
        private static string _nowToken = "";

        private static string _nowLoginCookieKey = "Login";

        public async override System.Threading.Tasks.Task ProcessRequestAsync(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string resposeStr = "";
            var action = context.Request[ParaForServerSdk.ActionKeyName];
            switch (action)
            {
                case ParaForServerSdk.ActionForAskYangAuthPush:
                    {
                        #region ActionForAskYangAuthPush

                        var thisRequestUserId = context.Request[ParaForServerSdk.UserIdKeyName];
                        if (!string.IsNullOrEmpty(thisRequestUserId))
                        {
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
                                    resposeStr = thisResponseServerSdkPush.Serialize();
                                }
                            }
                        } 

                        #endregion
                    }
                    break;
                case ParaForServerSdk.ActionForGetYcAuthQrCode:
                    {
                        #region ActionForGetYcAuthQrCode

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
                                resposeStr = thisResponseServerSdkQrCode.Serialize();
                            }
                        } 

                        #endregion
                    }
                    break;
                case ParaForServerSdk.ActionForCheckYcAuthResult:
                    {
                        #region ActionForCheckYcAuthResult

                        var thisRequestEventId = context.Request[ParaForServerSdk.EventIdKeyName];
                        if (!string.IsNullOrEmpty(thisRequestEventId))
                        {
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
                                        _nowToken = Guid.NewGuid().ToString();
                                        context.Response.SetCookie(new HttpCookie(_nowLoginCookieKey, _nowToken));
                                        resposeStr = thisResponseServerSdkResult.Serialize();
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
                        } 

                        #endregion
                    }
                    break;
                case ParaForServerSdk.ActionForYangAuthTokenLogin:
                    {
                        #region ActionForYangAuthTokenLogin

                        var thisRequestUserName = context.Request[ParaForServerSdk.UserNameKeyName];
                        var thisRequestAuthToken = context.Request[ParaForServerSdk.AuthTokenKeyName];
                        if (!string.IsNullOrEmpty(thisRequestUserName) && !string.IsNullOrEmpty(thisRequestAuthToken))
                        {
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
                                    resposeStr = new ResponseForServerSdkLogin
                                    {
                                        UserName = thisRequestUserName,
                                        UserToken = Guid.NewGuid().ToString().Replace("-", ""),
                                        Status = MsgForServerSdk.ApiCodeAtRightForReturn
                                    }.Serialize();
                                }
                            }
                        } 

                        #endregion
                    }
                    break;
            }

            context.Response.Write(resposeStr);
        }
    }

}