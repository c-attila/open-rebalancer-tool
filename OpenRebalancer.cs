using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;

namespace Rebalance_API
{
    public class Ticker
    {
        public string Symbol { get; set; }
        public string Price { get; set; }

        public new string ToString()
        {
            return String.Format("Symbol: {0}\tPrice: {1}", Symbol, Price);
        }
    }

    public class SnapshotResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("snapshotVos")]
        public IList<Snapshot> Snapshots { get; set; }

        public new string ToString()
        {
            string snapshotListString = "";
            foreach (Snapshot s in Snapshots)
            {
                snapshotListString += s.ToString();
            }
            return String.Format("Code: {0}\tMessage: {1}\tSnapshots: {2}", Code, Msg, snapshotListString);
        }


        public SnapshotResponse()
        {
            Code = -1;
            Msg = "Undefined";
            Snapshots = new List<Snapshot>() { new Snapshot() };
        }
    }

    public class Snapshot
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        public new string ToString()
        {
            return String.Format("Type: {0}\tUpdate time: {1}\tData: {2}\n", Type, UpdateTime, Data.ToString());
        }

        public Snapshot()
        {
            Type = "";
            UpdateTime = 0;
            Data = new Data();
        }
    }

    public class Data
    {
        [JsonProperty("totalAssetOfBtc")]
        public double TotalAssetOfBtc { get; set; }

        [JsonProperty("balances")]
        public IList<Balance> Balances { get; set; }

        public new string ToString()
        {
            string balanceListString = "";
            foreach (Balance b in Balances)
            {
                balanceListString += b.ToString();
            }
            return String.Format("Total asset of BTC: {0}\tBalances: {1}", TotalAssetOfBtc, balanceListString);
        }

        public Data()
        {
            TotalAssetOfBtc = 0;
            Balances = new List<Balance>() { new Balance() };
        }
    }

    public class Balance
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("free")]
        public float Free { get; set; }

        [JsonProperty("locked")]
        public float Locked { get; set; }

        public new string ToString()
        {
            return String.Format("Asset: {0}\tFree: {1}\tLocked: {2}\n", Asset, Free, Locked);
        }

        public Balance()
        {
            Asset = "";
            Free = 0;
            Locked = 0;
        }
    }

    public class SpotAccountInformation
    {
        [JsonProperty("makerCommision")]
        public int MakerCommision { get; set; }

        [JsonProperty("takerCommision")]
        public int TakerCommision { get; set; }

        [JsonProperty("buyerCommision")]
        public int BuyerCommision { get; set; }

        [JsonProperty("sellerCommision")]
        public int SellerCommision { get; set; }

        [JsonProperty("canTrade")]
        public bool CanTrade { get; set; }

        [JsonProperty("canWithdraw")]
        public bool CanWithdraw { get; set; }

        [JsonProperty("canDeposit")]
        public bool CanDeposit { get; set; }

        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        [JsonProperty("balances")]
        public IList<Balance> Balances = new List<Balance>();

        [JsonProperty("permissions")]
        public List<string> Permissions = new List<string>();

        public override string ToString()
        {
            string balanceListString = "";
            foreach (Balance b in Balances)
            {
                balanceListString += b.ToString();
            }
            return String.Format("Update time: {0}\nBalances: {1}\n", UpdateTime, balanceListString);
        }
    }

    public class ErrorResponse
    {
        public int code { get; set; }
        public string msg { get; set; }

        public new string ToString()
        {
            return code + ", " + msg;
        }
    }

    public class ErrorResponseException : Exception
    {
        ErrorResponse ErrorResponse { get; set; }

        public ErrorResponseException(ErrorResponse errorResponse) : base(errorResponse.ToString()) { this.ErrorResponse = ErrorResponse; }
    }

    public class Fill
    {
        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("qty")]
        public string Quantity { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("commissionAsset")]
        public string CommissionAsset { get; set; }
    }

    public class FilledOrderResponse
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("OrderListId")]
        public int OrderListId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("transactTime")]
        public long TransactTime { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("origQty")]
        public string OrigQty { get; set; }

        [JsonProperty("executedQty")]
        public string ExecutedQty { get; set; }

        [JsonProperty("cummulativeQuoteQty")]
        public string CummulativeQuoteQty { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("filles")]
        public IList<Fill> Fills { get; set; }
    }

    class OpenRebalancer
    {
        static SpotAccountInformation spotAccountInformation;
        private static string apiKey;
        private static string secretKey;
        private static string primaryAsset;
        private static string secondaryAsset;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No parameters are provided. \nPlease enter them now in the following format: [apiKey] [secretKey] [primary asset] [seconday asset] [asset1] [target1] [asset2] [target2]..");
                string inputText = Console.ReadLine();
                args = inputText.Split(' ');
            }

            try
            {
                apiKey = args[0];
                secretKey = args[1];
            } catch (ArgumentNullException)
            {
                Console.WriteLine("Not all keys are provided.");
                Environment.Exit(-1);
            }

            try
            {
                primaryAsset = args[2];
                secondaryAsset = args[3];
            } catch (ArgumentNullException)
            {
                Console.WriteLine("The primary or secondary asset is not provided.");
                Environment.Exit(-1);
            }

            spotAccountInformation = GetSpotAccountInformation();

            IList<string> trackedAssets = new List<string>();
            IDictionary<string, float> targetDistribution = new Dictionary<string, float>();

            if ((args.Length % 2) != 0 || args.Length == 2)
            {
                Console.Write("The list of distribution targets is incomplete or empty. Entered list: ");
                for (int i = 2; i <= args.Length; i++)
                    Console.Write(args[i] + " ");
                Environment.Exit(-1);
            }

            for (int i = 4; i < args.Length; i += 2)
            {
                trackedAssets.Add(args[i].ToString());
                targetDistribution.Add(args[i].ToString(), float.Parse(args[i + 1])/100);
            }

            Console.WriteLine("Initial balances in Bitcoin: ");
            IDictionary<string, float> assetBalancesInBitcoin = new Dictionary<string, float>();
            foreach (string asset in trackedAssets)
                assetBalancesInBitcoin.Add(asset, GetValueInBitcoin(asset));
            foreach (KeyValuePair<string, float> kvp in assetBalancesInBitcoin)
                Console.WriteLine(kvp.Key + " : " + kvp.Value);
            Console.WriteLine();

            Console.WriteLine("Distribution table: ");
            Console.WriteLine("Asset: target, current percentage -> deviation(Δ)");
            float totalBitcoinValue = assetBalancesInBitcoin.Sum(x => x.Value);

            IDictionary<string, Tuple<float, float>> distribuitonTable = new Dictionary<string, Tuple<float, float>>();
            foreach (string asset in trackedAssets)
                distribuitonTable.Add(asset, new Tuple<float, float>(targetDistribution[asset], ((float)assetBalancesInBitcoin[asset]) / totalBitcoinValue));
            foreach (KeyValuePair<string, Tuple<float, float>> kvp in distribuitonTable)
                Console.WriteLine(kvp.Key + " : " + kvp.Value.ToString() + " -> " + (kvp.Value.Item2 - kvp.Value.Item1)*100 + "%");
            Console.WriteLine("Total balance in Bitcoin: " + totalBitcoinValue);

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            Console.WriteLine();

            foreach (string asset in trackedAssets)
                HandleDifference(asset, ((distribuitonTable[asset].Item2 - distribuitonTable[asset].Item1) * totalBitcoinValue));

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static async Task<Ticker> GetTickerAsync(string pair)
        {
            var client = new RestClient("https://api.binance.com/");
            var request = new RestRequest("api/v3/ticker/price", Method.GET);
            request.AddParameter("symbol", pair);
            IRestResponse response = await client.ExecuteAsync(request);
            var content = response.Content;
            Ticker ticker = JsonConvert.DeserializeObject<Ticker>(content);
            return ticker;
        }

        static async Task<SnapshotResponse> GetSnapshotResponseAsync()
        {
            var client = new RestClient("https://api.binance.com/");
            var request = new RestRequest("/sapi/v1/accountSnapshot", Method.GET);
            AddSnapshotRequestParams(ref request);
            request.AddHeader("X-MBX-APIKEY", apiKey);
            IRestResponse response = await client.ExecuteAsync(request);
            var content = response.Content;
            SnapshotResponse snapshotResponse = JsonConvert.DeserializeObject<SnapshotResponse>(content);
            return snapshotResponse;
        }

        static string GetHMAC(string message, String secret)
        {
            Encoding ascii = Encoding.ASCII;
            HMACSHA256 hmac = new HMACSHA256(ascii.GetBytes(secret));
            byte[] hash = hmac.ComputeHash(ascii.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        static void AddSnapshotRequestParams(ref RestRequest request)
        {
            const string type = "SPOT";
            request.AddParameter("type", type);
            long timestamp = GetTimestamp() - 2000;
            request.AddParameter("timestamp", timestamp);
            const int recvWindow = 60000;
            request.AddParameter("recvWindow", recvWindow);
            string parameterlist = "type=" + type + "&timestamp=" + timestamp + "&recvWindow=" + recvWindow;
            string signature = GetHMAC(parameterlist, secretKey);
            request.AddParameter("signature", signature);
        }

        static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
        }

        static Snapshot GetMostRecentSnapshot()
        {
            IList<Snapshot> snapshotList = GetSnapshotResponseAsync().Result.Snapshots;
            Snapshot mostRecentSnapshot = new Snapshot();
            foreach (Snapshot snapshot in snapshotList)
            {
                if (snapshot.UpdateTime > mostRecentSnapshot.UpdateTime || snapshot.UpdateTime != 0)
                    mostRecentSnapshot = snapshot;
            }
            return mostRecentSnapshot;
        }

        static Snapshot GetNearestSnapshot(long timestamp)
        {
            IList<Snapshot> snapshotList = GetSnapshotResponseAsync().Result.Snapshots;
            Snapshot mostRecentSnapshot = new Snapshot();
            long lowestDifference = long.MaxValue;
            foreach (Snapshot snapshot in snapshotList)
            {
                if (Math.Abs(snapshot.UpdateTime - timestamp) < lowestDifference && snapshot.UpdateTime != 0)
                {
                    mostRecentSnapshot = snapshot;
                    lowestDifference = Math.Abs(snapshot.UpdateTime - timestamp);
                }
            }
            return mostRecentSnapshot;
        }

        static SpotAccountInformation GetSpotAccountInformation()
        {
            var client = new RestClient("https://api.binance.com/");
            var request = new RestRequest("/api/v3/account", Method.GET);
            AddSpotAccountRequestParams(ref request);
            request.AddHeader("X-MBX-APIKEY", apiKey);

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Unsuccesful account information GET request.");
                // throw new 
                Environment.Exit(-2);
            }
            var content = response.Content;
            SpotAccountInformation spotAccountInformation = JsonConvert.DeserializeObject<SpotAccountInformation>(content);
            return spotAccountInformation;
        }

        static void AddSpotAccountRequestParams(ref RestRequest request)
        {
            long timestamp = GetTimestamp() - 2000;
            request.AddParameter("timestamp", timestamp);
            string parameterlist = "timestamp=" + timestamp;
            string signature = GetHMAC(parameterlist, secretKey);
            request.AddParameter("signature", signature);
        }

        static IDictionary<string, Tuple<float, float>> UpdateDistributionTable(IList<string> trackedAssets, IDictionary<string, float> targetDistribution) {

            spotAccountInformation = GetSpotAccountInformation();

            Console.WriteLine("Initial balances in Bitcoin: ");
            IDictionary<string, float> assetBalancesInBitcoin = new Dictionary<string, float>();
            foreach (string asset in trackedAssets)
                assetBalancesInBitcoin.Add(asset, GetValueInBitcoin(asset));
            foreach (KeyValuePair<string, float> kvp in assetBalancesInBitcoin)
                Console.WriteLine(kvp.Key + " : " + kvp.Value);
            Console.WriteLine();

            Console.WriteLine("Distribution table: ");
            float totalBitcoinValue = assetBalancesInBitcoin.Sum(x => x.Value);

            IDictionary<string, Tuple<float, float>> distribuitonTable = new Dictionary<string, Tuple<float, float>>();
            foreach (string asset in trackedAssets)
                distribuitonTable.Add(asset, new Tuple<float, float>(targetDistribution[asset], ((float)assetBalancesInBitcoin[asset]) / totalBitcoinValue));

            return distribuitonTable;
        }

        static Balance GetBalance(string asset)
        {
            foreach (Balance b in spotAccountInformation.Balances)
                if (b.Asset.Equals(asset))
                    return b;
            return new Balance();
        }

        static float GetValueInBitcoin(string asset)
        {
            if (asset.Equals("BTC"))
                return 1 * GetBalance(asset).Free;
            if (asset.Equals("EUR"))
                return GetBalance(asset).Free / GetBitcoinPrice(asset);
            return GetBitcoinPrice(asset) * GetBalance(asset).Free;
        }

        static float GetBitcoinPrice(string asset)
        {
            if (asset.Equals("EUR"))
                return float.Parse(GetTickerAsync(string.Concat("BTC", asset)).Result.Price);
            return float.Parse(GetTickerAsync(string.Concat(asset, "BTC")).Result.Price);
        }

        static void HandleDifference(string asset, float quantity)
        {
            Console.WriteLine("-----Handling difference for " + asset + "-----");

            string side;
            if (quantity < 0)
            {
                side = "BUY";
                quantity *= -1;
            }
            else side = "SELL";

            if (side.Equals("BUY"))
            {
                try
                {
                    Trade(side, asset + primaryAsset, "quantity", GetAssetValue(asset, quantity));
                }
                catch (ErrorResponseException e)
                {
                    Console.WriteLine("Error message: " + e.Message);
                    try
                    {
                        Trade(side, asset + secondaryAsset, "quoteOrderQty", quantity);
                    }
                    catch (ErrorResponseException nestedE)
                    {
                        Console.WriteLine("Error message: " + nestedE.Message);
                    }
                }
            }
            else
            {
                try
                {
                    Trade(side, asset + secondaryAsset, "quoteOrderQty", quantity);
                }
                catch (ErrorResponseException e)
                {
                    Console.WriteLine("Error message: " + e.Message);
                    try
                    {
                        Trade(side, asset + primaryAsset, "quantity", GetAssetValue(asset, quantity));
                    }
                    catch (ErrorResponseException nestedE)
                    {
                        Console.WriteLine("Error message: " + nestedE.Message);
                    }
                }
            }

            Console.WriteLine();
        }

        static void Trade(string side, string pair, string quantityParam, float quantity)
        {
            var client = new RestClient("https://api.binance.com/");
            IRestResponse response;
            var request = new RestRequest("/api/v3/order/test", Method.POST);
            request.AddParameter("symbol", pair);
            request.AddParameter("side", side);
            request.AddParameter("type", "MARKET");
            request.AddParameter(quantityParam, Math.Round(quantity, 8).ToString("N8"));
            request.AddParameter("timestamp", GetTimestamp() - 2000);
            request.AddParameter("recvWindow", 60000);
            string parameterlist = "";
            foreach (Parameter p in request.Parameters)
                parameterlist = parameterlist + p.ToString() + "&";
            parameterlist = parameterlist.Substring(0, parameterlist.Length - 1);
            request.AddParameter("signature", GetHMAC(parameterlist, secretKey));
            request.AddHeader("X-MBX-APIKEY", apiKey);
            response = client.Execute(request);
            var content = response.Content;
            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
            if (errorResponse.code < 0)
            {
                Console.WriteLine("Failed order: Side: {0}; Pair: {1}; {2}: {3}", side, pair, quantityParam, quantity.ToString("N8"));
                throw new ErrorResponseException(errorResponse);
            }
            else
            {
                Console.WriteLine("Order passed filter: Side: {0}; Pair: {1}; {2}: {3}.. Do you want to continue? (y/n)", side, pair, quantityParam, quantity.ToString("N8"));
                if (Console.ReadLine().Equals("y"))
                {
                    request.Resource = "/api/v3/order";
                    response = client.Execute(request);
                    content = response.Content;
                    errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                    if (errorResponse.code < 0)
                    {
                        Console.WriteLine("Failed order: Side: {0}; Pair: {1}; {2}: {3}", side, pair, quantityParam, quantity.ToString("N8"));
                        throw new ErrorResponseException(errorResponse);
                    }
                    else
                    {
                        spotAccountInformation = GetSpotAccountInformation();
                    }
                }
                else
                {
                    Console.WriteLine("Order aborted.");
                    return;
                }
                Console.WriteLine("Successful order: Side: {0}; Pair: {1}; {2}: {3}", side, pair, quantityParam, quantity.ToString("N8"));
            }
        }

        static float GetAssetValue(string asset, float quantity)
        {
            if (asset.Equals("BTC"))
                return quantity;
            return quantity / GetBitcoinPrice(asset);
        }
    }
}

