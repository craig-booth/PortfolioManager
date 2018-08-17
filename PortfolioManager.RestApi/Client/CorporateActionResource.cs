using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using PortfolioManager.Common;
using PortfolioManager.RestApi.CorporateActions;

namespace PortfolioManager.RestApi.Client
{
    public class CorporateActionResource : RestResource
    {
        public CorporateActionResource(HttpClient httpClient)
            : base(httpClient)
        {

        }

        public async Task<IEnumerable<CorporateActionResponse>> GetAll(Guid stockid)
        {
            return await GetAsync<IEnumerable<CorporateActionResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions");
        }

        public async Task<IEnumerable<CorporateActionResponse>> GetAll(Guid stockid, DateRange dateRange)
        {
            return await GetAsync<IEnumerable<CorporateActionResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions?fromdate=" + dateRange.FromDate.ToString("yyyy-MM-dd") + "?todate=" + dateRange.ToDate.ToString("yyyy-MM-dd"));
        }

        public async Task<CorporateActionResponse> Get<CorporateActionResponse>(Guid stockid, Guid id)
        {
            return await GetAsync<CorporateActionResponse>("/api/stocks/" + stockid.ToString() + "/corporateactions/" + id.ToString());
        }

        public async Task<IEnumerable<DividendResponse>> GetDividends(Guid stockid)
        {
            return await GetAsync<IEnumerable<DividendResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions/dividends");
        }

        public async Task<IEnumerable<DividendResponse>> GetDividends(Guid stockid, DateRange dateRange)
        {
            return await GetAsync<IEnumerable<DividendResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions/dividends?fromdate=" + dateRange.FromDate.ToString("yyyy-MM-dd") + "?todate=" + dateRange.ToDate.ToString("yyyy-MM-dd"));
        }

        public async Task<DividendResponse> GetDividend(Guid stockid, Guid id)
        {
            return await GetAsync<DividendResponse>("/api/stocks/" + stockid.ToString() + "/corporateactions/dividends" + id.ToString());
        }

        public async Task AddDividend(AddDividendCommand command)
        {
            await PostAsync<AddDividendCommand>("/api/stocks/" + command.Stock.ToString() + "/corporateactions/dividends", command);
        }


        public async Task<IEnumerable<CapitalReturnResponse>> GetCapitalReturns(Guid stockid)
        {
            return await GetAsync<IEnumerable<CapitalReturnResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions/capitalreturns");
        }

        public async Task<IEnumerable<CapitalReturnResponse>> GetCapitalReturns(Guid stockid, DateRange dateRange)
        {
            return await GetAsync<IEnumerable<CapitalReturnResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions/capitalreturns?fromdate=" + dateRange.FromDate.ToString("yyyy-MM-dd") + "?todate=" + dateRange.ToDate.ToString("yyyy-MM-dd"));
        }

        public async Task<CapitalReturnResponse> GetCapitalReturn(Guid stockid, Guid id)
        {
            return await GetAsync<CapitalReturnResponse>("/api/stocks/" + stockid.ToString() + "/corporateactions/capitalreturns" + id.ToString());
        }

        public async Task AddCapitalReturn(AddCapitalReturnCommand command)
        {
            await PostAsync<AddCapitalReturnCommand>("/api/stocks/" + command.Stock.ToString() + "/corporateactions/capitalreturns", command);
        }


        public async Task<IEnumerable<TransformationResponse>> GetTransformations(Guid stockid)
        {
            return await GetAsync<IEnumerable<TransformationResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions/transformations");
        }

        public async Task<IEnumerable<TransformationResponse>> GetTransformations(Guid stockid, DateRange dateRange)
        {
            return await GetAsync<IEnumerable<TransformationResponse>>("/api/stocks/" + stockid.ToString() + "/corporateactions/transformations?fromdate=" + dateRange.FromDate.ToString("yyyy-MM-dd") + "?todate=" + dateRange.ToDate.ToString("yyyy-MM-dd"));
        }

        public async Task<TransformationResponse> GetTransformation(Guid stockid, Guid id)
        {
            return await GetAsync<TransformationResponse>("/api/stocks/" + stockid.ToString() + "/corporateactions/transformations" + id.ToString());
        }

        public async Task AddTransformation(AddTransformationCommand command)
        {
            await PostAsync<AddTransformationCommand>("/api/stocks/" + command.Stock.ToString() + "/corporateactions/transformations", command);
        }
    }
}
