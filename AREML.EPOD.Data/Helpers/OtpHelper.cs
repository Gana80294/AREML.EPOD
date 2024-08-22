using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Data.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace AREML.EPOD.Data.Helpers
{
    public class OtpHelper
    {
        private readonly OtpSetting _otpSetting;

        public OtpHelper(IConfiguration config)
        {
            _otpSetting = config.GetSection("SmsConfig").Get<OtpSetting>();
        }

        public async Task<dynamic> SendOTPAsync(User user)
        {
            try
            {
                var httpClient = new HttpClient();
                string randomOtp = new Random().Next(0, 1000000).ToString("D6");
                string smsMessage = _otpSetting.smsMsg
                    .Replace("U$erN@me", user.UserName.Substring(0, 3))
                    .Replace("(o%^!)", randomOtp);

                var otpPayload = new SendOtpMtalkZPayload
                {
                    apikey = _otpSetting.apiKey,
                    senderid = _otpSetting.senderID,
                    number = "91" + user.ContactNumber,
                    message = smsMessage,
                    format = "json"
                };

                var payloadJson = JsonSerializer.Serialize(otpPayload);

                //set the request content
                var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

                //call the api
                var response = await httpClient.PostAsync(_otpSetting.otpRequestApi, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Invalid response received from the server. HTTP error code: {response.StatusCode}");
                }

                // If a valid response is received, get the JSON response
                var responseBody = await response.Content.ReadAsStringAsync();
                var otpResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                var result = new
                {
                    Status = otpResponse["status"],
                    Message = otpResponse["message"],
                    Otp = randomOtp,
                    MsgId = otpResponse["msgid"]
                };

                return result;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("OtpHelper/SendOTPAsync/Exception :- ", ex);
                throw ex;
            }
        }


        //public async Task<dynamic> SendOTP(User user)
        //{
        //    try
        //    {
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_otpSetting.otpRequestApi);
        //        request.Method = "POST";
        //        request.KeepAlive = true;
        //        request.AllowAutoRedirect = true;
        //        request.Accept = "*/*";
        //        Random generator = new Random();
        //        String r = generator.Next(0, 1000000).ToString("D6");
        //        string smsotp = "";
        //        smsotp = _otpSetting.smsMsg.Replace("U$erN@me", user.UserName.Substring(0, 3)).Replace("(o%^!)", r);

        //        SendOtpMtalkZPayload SendOTPPayload = new SendOtpMtalkZPayload
        //        {
        //            apikey = _otpSetting.apiKey,
        //            senderid = _otpSetting.senderID,
        //            number = "91" + user.ContactNumber,
        //            message = smsotp,
        //            format = "json"
        //        };

        //        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
        //        {
        //            var payload = JsonSerializer.Serialize(SendOTPPayload);
        //            writer.Write(payload);
        //            writer.Flush();
        //            writer.Close();
        //        }

        //        string response = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
        //        LogWriter.WriteToFile($"OtpHelper/SendOTP/response :- {response}");
        //        dynamic otpResponse = JsonSerializer.Deserialize<dynamic>(response);

        //        var res = new
        //        {
        //            Status = otpResponse.status,
        //            Message = otpResponse.message,
        //            Otp = r,
        //            MsgId = otpResponse.msgid
        //        };
        //        return res;
        //    }
        //    catch(Exception ex)
        //    {
        //        LogWriter.WriteToFile("OtpHelper/SendOTP/Exception :- ", ex);
        //        throw ex;
        //    }
        //}
    }
}
