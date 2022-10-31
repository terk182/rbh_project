using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BL.Entities.CurrencyExchange;
using DataModel.UnitOfWork;
using Newtonsoft.Json;

namespace BL
{
    public class CurrencyExchange : ICurrencyExchange
    {
        private readonly UnitOfWork _unitOfWork;

        public CurrencyExchange(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public decimal ConvertFromTHB(string toCCY, decimal amount)
        {
            if (toCCY.ToUpper() == "THB")
            {
                return amount;
            }
            List<FXRate> fxRates = this.FetchRate();
            decimal toAmount = 9999999;
            var rate = fxRates.FirstOrDefault(x => x.currencyCode.ToUpper() == toCCY.ToUpper());
            if (rate != null)
            {
                toAmount = amount / rate.rateToTHB;
            }
            if (toCCY.ToUpper() == "JPY")
            {
                toAmount *= 100;
            }
            if (toCCY.ToUpper() == "IDR")
            {
                toAmount *= 1000;
            }
            return decimal.Round(toAmount, 2, MidpointRounding.AwayFromZero);
        }

        public decimal ConvertToTHB(string fromCCY, decimal amount)
        {
            if (fromCCY.ToUpper() == "THB")
            {
                return amount;
            }
            List<FXRate> fxRates = this.FetchRate();
            decimal toAmount = 9999999;
            var rate = fxRates.FirstOrDefault(x => x.currencyCode.ToUpper() == fromCCY.ToUpper());
            if (rate != null)
            {
                toAmount = amount * rate.rateToTHB;
            }

            if (fromCCY.ToUpper() == "JPY")
            {
                toAmount /= 100;
            }
            if (fromCCY.ToUpper() == "IDR")
            {
                toAmount /= 1000;
            }
            toAmount = toAmount * 1.025M; //Risk rate
            return decimal.Round(toAmount, 2, MidpointRounding.AwayFromZero);
        }

        public List<FXRate> FetchRate()
        {
            List<FXRate> fxRates = new List<FXRate>();
            Utilities.MemoryCacher cacher = new Utilities.MemoryCacher();
            if (cacher.GetValue("C_FXRATE") != null)
            {
                fxRates = (List<FXRate>)cacher.GetValue("C_FXRATE");
            }
            else
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                DateTime today = DateTime.Today;
                var todayRate = _unitOfWork.CurrencyExchangeRepository.GetFirstOrDefault(x => x.TimeStamp.GetValueOrDefault().Date == today);
                if (todayRate == null)
                {
                    //fetch new data
                    for(int i = 1; i <= 5; i++)
                    {
                        try
                        {
                            DateTime checkDate = today.AddDays(-1 * i);
                            string url = "https://apigw1.bot.or.th/bot/public/Stat-ExchangeRate/v2/DAILY_AVG_EXG_RATE/?start_period={0}&end_period={1}";
                            url = String.Format(url, checkDate.ToString("yyyy-MM-dd"), checkDate.ToString("yyyy-MM-dd"));
                            string json = "";
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            request.ContentType = "application/json; charset=utf-8";
                            request.Headers.Add("x-ibm-client-id", "d227e0c6-aa18-4bd2-8162-b826ac03bd14");
                            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                                json = reader.ReadToEnd();
                            }

                            var botFx = Entities.CurrencyExchange.BotResponse.FromJson(json);
                            var checkRate = botFx.Result.Data.DataDetail.FirstOrDefault(x => x.CurrencyId == "USD");
                            if (checkRate != null && checkRate.MidRate != "")
                            {
                                foreach(var rate in botFx.Result.Data.DataDetail)
                                {
                                    FXRate fx = new FXRate();
                                    fx.currencyCode = rate.CurrencyId;
                                    fx.currencyName = rate.CurrencyNameEng;
                                    fx.rateToTHB = Convert.ToDecimal(rate.MidRate);
                                    if (fx.currencyCode == "JPY")
                                    {
                                        fx.currencyName = fx.currencyName.Replace("(100 YEN)", "").Trim();
                                        fx.rateToTHB = fx.rateToTHB * 0.01M;
                                    }
                                    if (fx.currencyCode == "IDR")
                                    {
                                        fx.currencyName = fx.currencyName.Replace("(1,000 RUPIAH)", "").Trim();
                                        fx.rateToTHB = fx.rateToTHB * 0.001M;
                                    }
                                    fxRates.Add(fx);
                                }
                                i = 6;
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    if (fxRates.Count > 0)
                    {
                        //save to db
                        string rateJson = JsonConvert.SerializeObject(fxRates);
                        DataModel.CurrencyExchange cEx = new DataModel.CurrencyExchange();
                        cEx.CurrencyExchangeOID = Guid.NewGuid();
                        cEx.ExchangeRate = rateJson;
                        cEx.TimeStamp = DateTime.Now;

                        _unitOfWork.CurrencyExchangeRepository.Insert(cEx);
                        _unitOfWork.Save();
                        cacher.Add("C_FXRATE", fxRates, DateTime.Now.AddHours(12));
                    }
                    else
                    {
                        //get latest rate
                        var latestRate = _unitOfWork.CurrencyExchangeRepository.GetAll().OrderByDescending(x => x.TimeStamp).FirstOrDefault();
                        if (latestRate != null)
                        {
                            string rateJson = latestRate.ExchangeRate;
                            fxRates = JsonConvert.DeserializeObject<List<FXRate>>(rateJson);
                        }
                    }
                }
                else
                {
                    string rateJson = todayRate.ExchangeRate;
                    fxRates = JsonConvert.DeserializeObject<List<FXRate>>(rateJson);
                }

            }

            return fxRates;
        }
    }
}
