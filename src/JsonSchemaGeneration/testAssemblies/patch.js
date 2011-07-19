
{
    "smd": {
        "GetChartingEnabled": "exclude",
        "GetTermsAndConditions": "exclude",
        "ListMarketInformationSearch": {
            "uriTemplate": "/informationsearch?SearchByMarketCode={searchByMarketCode}&SearchByMarketName={searchByMarketName}&SpreadProductType={spreadProductType}&CfdProductType={cfdProductType}&BinaryProductType={binaryProductType}&Query={query}&MaxResults={maxResults}"
        },
        "ListOpenPositions": {
            "uriTemplate": "/openpositions?TradingAccountId={tradingAccountId}"
        },
        "ListActiveStopLimitOrders": {
            "uriTemplate": "/activestoplimitorders?TradingAccountId={tradingAccountId}"
        },
        "ListTradeHistory": {
            "uriTemplate": "/tradehistory?TradingAccountId={tradingAccountId}&MaxResults={maxResults}"
        },
        "ListStopLimitOrderHistory": {
            "uriTemplate": "/stoplimitorderhistory?TradingAccountId={tradingAccountId}&MaxResults={maxResults}"
        }
    },
    "schema": {}
};