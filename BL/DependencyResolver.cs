using System.ComponentModel.Composition;
using Resolver;

namespace BL
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IAPIConfigServices, APIConfigServices>();
            registerComponent.RegisterType<IAirPromotionServices, AirPromotionServices>();
            registerComponent.RegisterType<IAirlineControlServices, AirlineControlServices>();
            registerComponent.RegisterType<IAirlineQtaxControlServices, AirlineQtaxControlServices>();
            registerComponent.RegisterType<IBackofficeAdminServices, BackofficeAdminServices>();
            registerComponent.RegisterType<IBackofficeAdminServices, BackofficeAdminServices>();
            registerComponent.RegisterType<ICRMServices, CRMServices>();
            registerComponent.RegisterType<ICurrencyExchange, CurrencyExchange>();
            registerComponent.RegisterType<IFlightBookingServices, FlightBookingServices>();
            registerComponent.RegisterType<IFlightSearchServices, FlightSearchServices>();
            registerComponent.RegisterType<IFlightReportServices, FlightReportServices>();
            registerComponent.RegisterType<IFlightReportRefundServices, FlightReportRefundServices>();
            registerComponent.RegisterType<IFlightReportReissueServices, FlightReportReissueServices>();
            registerComponent.RegisterType<IFourDigitControlServices, FourDigitControlServices>();
            registerComponent.RegisterType<IFlightSearchServices, FlightSearchServices>(); ;
            registerComponent.RegisterType<IMarkupServices, MarkupServices>();
            registerComponent.RegisterType<IPassportServices, PassportServices>();
            registerComponent.RegisterType<IKBankChargeServices, KBankChargeServices>();
            registerComponent.RegisterType<INamingServices, NamingServices>();
            registerComponent.RegisterType<IPaymentServices, PaymentServices>();
            registerComponent.RegisterType<IRunningNumberServices, RunningNumberServices>();
            registerComponent.RegisterType<ISeatMapControlServices, SeatMapControlServices>();
            registerComponent.RegisterType<ISendEmailServices, SendEmailServices>();
            registerComponent.RegisterType<IFlightFareRuleConfigServices, FlightFareRuleConfigServices>();
            registerComponent.RegisterType<IAirlineConfigServices, AirlineConfigServices>();
            registerComponent.RegisterType<ISiteConfigServices, SiteConfigServices>();
            registerComponent.RegisterType<IDiscountTagServices, DiscountTagServices>();
            registerComponent.RegisterType<IPromotionGroupCodeServices, PromotionGroupCodeServices>();
            registerComponent.RegisterType<ILionAirServices, LionAirServices>();
        }
    }
}