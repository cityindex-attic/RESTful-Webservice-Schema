{
  "SMDVersion": "2.6",
  "version": "1",
  "description": "CIAPI SMD",
  "services": {
    "rpc": {
      "target": "",
      "services": {
        "LogOn": {
          "description": "Create a new session. This is how you \"log on\" to the CIAPI.",
          "target": "session",
          "uriTemplate": "/",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiLogOnResponseDTO"
          },
          "group": "Authentication",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.ApiLogOnRequestDTO",
              "name": "apiLogOnRequest",
              "description": "The request to create a session (log on)."
            }
          ]
        },
        "DeleteSession": {
          "description": "<p>Delete a session. This is how you \"log off\" from the CIAPI.</p>",
          "target": "session",
          "uriTemplate": "/deleteSession?userName={userName}&session={session}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiLogOffResponseDTO"
          },
          "group": "Authentication",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "userName",
              "description": "Username is case sensitive. May be set as a service parameter or as a request header.",
              "demoValue": "CC735158",
              "minLength": 6,
              "maxLength": 20
            },
            {
              "type": "string",
              "name": "session",
              "description": "The session token. May be set as a service parameter or as a request header.",
              "demoValue": "5998CBE8-3594-4232-A57E-09EC3A4E7AA8",
              "format": "guid",
              "minLength": 36,
              "maxLength": 36
            }
          ]
        },
        "ChangePassword": {
          "description": "Change a user's password.",
          "target": "session",
          "uriTemplate": "/changePassword",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiChangePasswordResponseDTO"
          },
          "group": "Authentication",
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.ApiChangePasswordRequestDTO",
              "name": "apiChangePasswordRequest",
              "description": "The change password request details."
            }
          ]
        },
        "GetPriceBars": {
          "description": "Get historic price bars for the specified market in OHLC (open, high, low, close) format, suitable for plotting in candlestick charts. Returns price bars in ascending order up to the current time. When there are no prices for a particular time period, no price bar is returned. Thus, it can appear that the array of price bars has \"gaps\", i.e. the gap between the date & time of each price bar might not be equal to interval x span. Sample Urls: <ul> <li>/market/1234/history?interval=MINUTE&span=15&pricebars=180</li> <li>/market/735/history?interval=HOUR&span=1&pricebars=240</li> <li>/market/1577/history?interval=DAY&span=1&pricebars=10</li> </ul>",
          "target": "market",
          "uriTemplate": "/{marketId}/barhistory?interval={interval}&span={span}&pricebars={priceBars}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetPriceBarResponseDTO"
          },
          "group": "Price History",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "marketId",
              "description": "The marketId.",
              "demoValue": "71442"
            },
            {
              "type": "string",
              "name": "interval",
              "description": "The pricebar interval.",
              "demoValue": "MINUTE"
            },
            {
              "type": "integer",
              "name": "span",
              "description": "The number of each interval per pricebar.",
              "demoValue": 1
            },
            {
              "type": "string",
              "name": "priceBars",
              "description": "The total number of pricebars to return.",
              "demoValue": "15"
            }
          ]
        },
        "GetPriceTicks": {
          "description": "Get historic price ticks for the specified market. Returns price ticks in ascending order up to the current time. The length of time that elapses between each tick is usually different.",
          "target": "market",
          "uriTemplate": "/{marketId}/tickhistory?priceticks={priceTicks}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetPriceTickResponseDTO"
          },
          "group": "Price History",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "marketId",
              "description": "The marketId.",
              "demoValue": "71442"
            },
            {
              "type": "string",
              "name": "priceTicks",
              "description": "The total number of price ticks to return.",
              "demoValue": "10"
            }
          ]
        },
        "ListNewsHeadlinesWithSource": {
          "description": "Get a list of current news headlines",
          "target": "news",
          "uriTemplate": "/{source}/{category}?MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListNewsHeadlinesResponseDTO"
          },
          "group": "News",
          "cacheDuration": 10000,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "source",
              "description": "The news feed source provider. Valid options are: dj|mni|ci.",
              "demoValue": "dj"
            },
            {
              "type": "string",
              "name": "category",
              "description": "Filter headlines by category. Valid categories depend on the source used:  for dj: uk|aus, for ci: SEMINARSCHINA, for mni: ALL.",
              "demoValue": "UK",
              "minLength": 2,
              "maxLength": 3
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "Specify the maximum number of headlines returned",
              "demoValue": 10,
              "default": 25,
              "minimum": 1,
              "maximum": 500
            }
          ]
        },
        "GetNewsDetail": {
          "description": "Get the detail of the specific news story matching the story Id in the parameter.",
          "target": "news",
          "uriTemplate": "/detail/{source}/{storyId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetNewsDetailResponseDTO"
          },
          "group": "News",
          "cacheDuration": 10000,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "source",
              "description": "The news feed source provider. Valid options are dj|mni|ci.",
              "demoValue": "dj"
            },
            {
              "type": "string",
              "name": "storyId",
              "description": "The news story Id.",
              "demoValue": "12654",
              "minLength": 1,
              "maxLength": 9
            }
          ]
        },
        "ListCfdMarkets": {
          "description": "Returns a list of CFD markets filtered by market name and/or market code. Leave the market name and code parameters empty to return all markets available to the User.",
          "target": "cfd/markets",
          "uriTemplate": "?MarketName={searchByMarketName}&MarketCode={searchByMarketCode}&ClientAccountId={clientAccountId}&MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListCfdMarketsResponseDTO"
          },
          "group": "CFD Markets",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "searchByMarketName",
              "description": "The characters that the CFD market name starts with (Optional).",
              "demoValue": "voda",
              "minLength": 1,
              "maxLength": 120
            },
            {
              "type": "string",
              "name": "searchByMarketCode",
              "description": "The characters that the market code starts with, normally this is the RIC code for the market (Optional).",
              "minLength": 1,
              "maxLength": 50
            },
            {
              "type": "integer",
              "name": "clientAccountId",
              "description": "The logged on user's ClientAccountId. This only shows you the markets that the user can trade. (Required).",
              "demoValue": 123456,
              "minimum": 1,
              "maximum": 2147483647,
              "required": true
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "The maximum number of markets to return.",
              "demoValue": 20,
              "minimum": 1,
              "maximum": 200,
              "default": 20
            }
          ]
        },
        "ListSpreadMarkets": {
          "description": "Returns a list of Spread Betting markets filtered by market name and/or market code. Leave the market name and code parameters empty to return all markets available to the User.",
          "target": "spread/markets",
          "uriTemplate": "?MarketName={searchByMarketName}&MarketCode={searchByMarketCode}&ClientAccountId={clientAccountId}&MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListSpreadMarketsResponseDTO"
          },
          "group": "Spread Markets",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "searchByMarketName",
              "description": "The characters that the Spread market name starts with (Optional).",
              "demoValue": "voda",
              "minLength": 1,
              "maxLength": 120
            },
            {
              "type": "string",
              "name": "searchByMarketCode",
              "description": "The characters that the Spread market code starts with, normally this is the RIC code for the market (Optional).",
              "demoValue": "VOD.L",
              "minLength": 1,
              "maxLength": 50
            },
            {
              "type": "integer",
              "name": "clientAccountId",
              "description": "The logged on user's ClientAccountId. (This only shows you markets that you can trade on.)",
              "demoValue": 123456,
              "minimum": 1,
              "maximum": 84272157
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "The maximum number of markets to return.",
              "demoValue": 20,
              "minimum": 1,
              "maximum": 500,
              "optional": true,
              "default": 20
            }
          ]
        },
        "GetMarketInformation": {
          "description": "<p>Get Market Information for the single specified market supplied in the parameter.</p>",
          "target": "market",
          "uriTemplate": "/{marketId}/information",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetMarketInformationResponseDTO"
          },
          "group": "Market",
          "cacheDuration": 1000,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "marketId",
              "description": "The marketId.",
              "demoValue": "71442"
            }
          ]
        },
        "ListMarketInformationSearch": {
          "description": "<p>Returns market information for the markets that meet the search criteria.</p> The search can be performed by market code and/or market name, and can include CFDs and Spread Bet markets.",
          "target": "market",
          "uriTemplate": "/market/informationsearch?SearchByMarketCode={searchByMarketCode}&SearchByMarketName={searchByMarketName}&SpreadProductType={spreadProductType}&CfdProductType={cfdProductType}&BinaryProductType={binaryProductType}&Query={query}&MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListMarketInformationSearchResponseDTO"
          },
          "group": "Market",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "boolean",
              "name": "searchByMarketCode",
              "description": "Sets the search to use market code.",
              "demoValue": true
            },
            {
              "type": "boolean",
              "name": "searchByMarketName",
              "description": "Sets the search to use market Name.",
              "demoValue": true
            },
            {
              "type": "boolean",
              "name": "spreadProductType",
              "description": "Sets the search to include spread bet markets.",
              "demoValue": true
            },
            {
              "type": "boolean",
              "name": "cfdProductType",
              "description": "Sets the search to include CFD markets.",
              "demoValue": true
            },
            {
              "type": "boolean",
              "name": "binaryProductType",
              "description": "Sets the search to include binary markets.",
              "demoValue": true
            },
            {
              "type": "string",
              "name": "query",
              "description": "The text to search for. Matches part of market name / code from the start.",
              "demoValue": "UK 100"
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "The maximum number of results to return.",
              "demoValue": 50
            }
          ]
        },
        "SearchWithTags": {
          "description": "Get market information and tags for the markets that meet the search criteria.",
          "target": "market",
          "uriTemplate": "/market/searchwithtags?Query={query}&TagId={tagId}&MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.MarketInformationSearchWithTagsResponseDTO"
          },
          "group": "Market",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "query",
              "description": "The text to search for. Matches part of market name / code from the start.",
              "demoValue": "UK 100"
            },
            {
              "type": "integer",
              "name": "tagId",
              "description": "The ID for the tag to be searched (optional).",
              "demoValue": 0
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "The maximum number of results to return.  Default is 20.",
              "demoValue": 50
            }
          ]
        },
        "TagLookup": {
          "description": "<p>Gets all of the tags the the requesting user is allowed to see. Tags are returned in a primary / secondary hierarchy.</p>",
          "target": "market",
          "uriTemplate": "/market/taglookup",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.MarketInformationTagLookupResponseDTO"
          },
          "group": "Market",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": []
        },
        "ListMarketInformation": {
          "description": "Get Market Information for the specified list of markets.",
          "target": "market",
          "uriTemplate": "/market/information",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ListMarketInformationResponseDTO"
          },
          "group": "Market",
          "cacheDuration": 1000,
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.ListMarketInformationRequestDTO",
              "name": "listMarketInformationRequestDTO",
              "description": "Get Market Information for the specified list of markets."
            }
          ]
        },
        "SaveMarketInformation": {
          "description": "Save Market Information for the specified list of markets.",
          "target": "market",
          "uriTemplate": "/market/information/save",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiSaveMarketInformationResponseDTO"
          },
          "group": "Market",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.SaveMarketInformationRequestDTO",
              "name": "listMarketInformationRequestSaveDTO",
              "description": "Save Market Information for the specified list of markets."
            }
          ]
        },
        "Order": {
          "description": "<p>Place an order on a particular market. <p>Do not set any order id fields when requesting a new order, the platform will generate them.</p>",
          "target": "order",
          "uriTemplate": "/newstoplimitorder",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiTradeOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "throttleScope": "trading",
          "parameters": [
            {
              "$ref": "#.NewStopLimitOrderRequestDTO",
              "name": "order",
              "description": "The order request."
            }
          ]
        },
        "CancelOrder": {
          "description": "<p>Cancel an order.",
          "target": "order",
          "uriTemplate": "/cancel",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiTradeOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "$ref": "#.CancelOrderRequestDTO",
              "name": "cancelOrder",
              "description": "The cancel order request."
            }
          ]
        },
        "UpdateOrder": {
          "description": "<p>Update an order (for adding a stop/limit or attaching an OCO relationship).",
          "target": "order",
          "uriTemplate": "/updatestoplimitorder",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiTradeOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "$ref": "#.UpdateStopLimitOrderRequestDTO",
              "name": "order",
              "description": "The update order request."
            }
          ]
        },
        "ListOpenPositions": {
          "description": "<p>Queries for a specified trading account's trades / open positions.</p> <p>This uri is intended to support a grid in a UI. One usage pattern is to subscribe to streaming orders, call this for the initial data to display in the grid, and call the HTTP service GetOpenPosition when you get updates on the order stream to get the updated data in this format.</p>",
          "target": "order",
          "uriTemplate": "/order/openpositions?TradingAccountId={tradingAccountId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListOpenPositionsResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "integer",
              "name": "tradingAccountId",
              "description": "The trading account to get orders for."
            }
          ]
        },
        "ListActiveStopLimitOrders": {
          "description": "<p>Queries for a specified trading account's active stop / limit orders.</p> <p>This uri is intended to support a grid in a UI. One usage pattern is to subscribe to streaming orders, call this for the initial data to display in the grid, and call the HTTP service GetActiveStopLimitOrder when you get updates on the order stream to get the updated data in this format.</p>",
          "target": "order",
          "uriTemplate": "/order/activestoplimitorders?TradingAccountId={tradingAccountId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListActiveStopLimitOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "integer",
              "name": "tradingAccountId",
              "description": "The trading account to get orders for."
            }
          ]
        },
        "GetActiveStopLimitOrder": {
          "description": "<p>Queries for an active stop limit order with a specified order id. It returns a null value if the order doesn't exist, or is not an active stop limit order.<p> <p>This uri is intended to support a grid in a UI. One usage pattern is to subscribe to streaming orders, call the HTTP service ListActiveStopLimitOrders for the initial data to display in the grid, and call this uri when you get updates on the order stream to get the updated data in this format.</p> <p>For a more comprehensive order response, see the HTTP service GetOrder",
          "target": "order",
          "uriTemplate": "/{orderId}/activestoplimitorder",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetActiveStopLimitOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "string",
              "name": "orderId",
              "description": "The requested order id."
            }
          ]
        },
        "GetOpenPosition": {
          "description": "<p>Queries for a trade / open position with a specified order id. It returns a null value if the order doesn't exist, or is not a trade / open position.</p> <p>This uri is intended to support a grid in a UI. One usage pattern is to subscribe to streaming orders, call the HTTP service ListOpenPositions for the initial data to display in the grid, and call this uri when you get updates on the order stream to get the updated data in this format.</p> <p>For a more comprehensive order response, see the HTTP service GetOrder",
          "target": "order",
          "uriTemplate": "/{orderId}/openposition",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetOpenPositionResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "string",
              "name": "orderId",
              "description": "The requested order id."
            }
          ]
        },
        "ListTradeHistory": {
          "description": "<p>Queries for a specified trading account's trade history. The result set will contain orders with a status of <b>(3 - Open, 9 - Closed)</b>, and includes <b>orders that were a trade / stop / limit order</b>.</p> <p>There's currently no corresponding GetTradeHistory (as with ListOpenPositions).",
          "target": "order",
          "uriTemplate": "/order/tradehistory?TradingAccountId={tradingAccountId}&MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListTradeHistoryResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "integer",
              "name": "tradingAccountId",
              "description": "The trading account to get orders for."
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "The maximum number of results to return."
            }
          ]
        },
        "ListStopLimitOrderHistory": {
          "description": "<p>Queries for a specified trading account's stop / limit order history. The result set includes <b>only orders that were originally stop / limit orders</b> that currently have one of the following statuses <b>(3 - Open, 4 - Cancelled, 5 - Rejected, 9 - Closed, 10 - Red Card)</b>. </p> <p>There's currently no corresponding GetStopLimitOrderHistory (as with ListActiveStopLimitOrders).</p>",
          "target": "order",
          "uriTemplate": "/order/stoplimitorderhistory?TradingAccountId={tradingAccountId}&MaxResults={maxResults}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListStopLimitOrderHistoryResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "integer",
              "name": "tradingAccountId",
              "description": "The trading account to get orders for."
            },
            {
              "type": "integer",
              "name": "maxResults",
              "description": "The maximum number of results to return."
            }
          ]
        },
        "GetOrder": {
          "description": "<p>Queries for an order by a specific order id.</p> <p>The current implementation only returns active orders (i.e. those with a status of <b>1 - Pending, 2 - Accepted, 3 - Open, 6 - Suspended, 8 - Yellow Card, 11 - Triggered</b>).</p>",
          "target": "order",
          "uriTemplate": "/{orderId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "string",
              "name": "orderId",
              "description": "The requested order id."
            }
          ]
        },
        "Trade": {
          "description": "<p>Place a trade on a particular market.</p> <p>Do not set any order id fields when requesting a new trade, the platform will generate them.</p>",
          "target": "order",
          "uriTemplate": "/newtradeorder",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiTradeOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "throttleScope": "trading",
          "parameters": [
            {
              "$ref": "#.NewTradeOrderRequestDTO",
              "name": "trade",
              "description": "The trade request."
            }
          ]
        },
        "UpdateTrade": {
          "description": "Update a trade (for adding a stop/limit etc).",
          "target": "order",
          "uriTemplate": "/updatetradeorder",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiTradeOrderResponseDTO"
          },
          "group": "Trades and Orders",
          "cacheDuration": 0,
          "throttleScope": "trading",
          "parameters": [
            {
              "$ref": "#.UpdateTradeOrderRequestDTO",
              "name": "update",
              "description": "The update trade request."
            }
          ]
        },
        "GetChartingEnabled": {
          "description": "Checks whether the supplied User Account is allowed to see Charting Data.",
          "target": "useraccount",
          "uriTemplate": "/UserAccount/{id}/ChartingEnabled",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "type": "boolean"
          },
          "group": "AccountInformation",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "string",
              "name": "id",
              "description": "The User Account ID to check."
            }
          ]
        },
        "GetClientAndTradingAccount": {
          "description": "Returns the User's ClientAccountId and a list of their TradingAccounts. There are no parameters for this call.",
          "target": "useraccount",
          "uriTemplate": "/UserAccount/ClientAndTradingAccount",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.AccountInformationResponseDTO"
          },
          "group": "AccountInformation",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": []
        },
        "SaveAccountInformation": {
          "description": "Saves the users account information.",
          "target": "useraccount",
          "uriTemplate": "/UserAccount/Save",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiSaveAccountInformationResponseDTO"
          },
          "group": "AccountInformation",
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.ApiSaveAccountInformationRequestDTO",
              "name": "saveAccountInformationRequest",
              "description": "Saves the users account information."
            }
          ]
        },
        "GetMessage": {
          "description": "",
          "target": "message",
          "uriTemplate": "/Message/{id}?language={language}&category={category}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "type": "string"
          },
          "group": "Messages",
          "cacheDuration": 3600000,
          "parameters": [
            {
              "type": "string",
              "name": "id"
            },
            {
              "type": "string",
              "name": "language"
            },
            {
              "type": "string",
              "name": "category"
            }
          ]
        },
        "GetMessagePopup": {
          "description": "",
          "target": "message",
          "uriTemplate": "/message/popup?language={language}&ClientAccountId={clientAccountId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.GetMessagePopupResponseDTO"
          },
          "group": "Messages",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "string",
              "name": "language"
            },
            {
              "type": "integer",
              "name": "clientAccountId"
            }
          ]
        },
        "AcceptOrRejectMessagePopupResponse": {
          "description": "",
          "target": "message",
          "uriTemplate": "/message/popupchoice?ClientAccountId={clientAccountId}&Accepted={accepted}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "group": "Messages",
          "cacheDuration": 0,
          "parameters": [
            {
              "type": "integer",
              "name": "clientAccountId"
            },
            {
              "type": "boolean",
              "name": "accepted"
            }
          ]
        },
        "GetSystemLookup": {
          "description": "Use the message lookup service to get localised textual names for the various status code & Ids returned by the API. For example, a query for OrderStatusReasons will contain text names for all the possible values of OrderStatusReason in the ApiOrderResponseDTO. You should only request the list once per session (for each entity you're interested in).",
          "target": "message",
          "uriTemplate": "/message/lookup?lookupEntityName={lookupEntityName}&cultureId={cultureId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ApiLookupResponseDTO"
          },
          "group": "Messages",
          "cacheDuration": 3600000,
          "parameters": [
            {
              "type": "string",
              "name": "lookupEntityName",
              "description": "The entity to lookup (eg OrderStatusReason, InstructionStatusReason, OrderApplicability or Culture)"
            },
            {
              "type": "integer",
              "name": "cultureId",
              "description": "The cultureId used to override the translated text description. (optional)"
            }
          ]
        },
        "GetClientApplicationMessageTranslation": {
          "description": "Use the message translation service to get client specific translated textual strings.",
          "target": "message",
          "uriTemplate": "/message/translation?clientApplicationId={clientApplicationId}&cultureId={cultureId}&accountOperatorId={accountOperatorId}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ApiClientApplicationMessageTranslationResponseDTO"
          },
          "group": "Messages",
          "cacheDuration": 3600000,
          "parameters": [
            {
              "type": "integer",
              "name": "clientApplicationId",
              "description": "Client application identifier. (optional)"
            },
            {
              "type": "integer",
              "name": "cultureId",
              "description": "CultureId which corresponds to a culture code. (optional)"
            },
            {
              "type": "integer",
              "name": "accountOperatorId",
              "description": "Account operator identifier. (optional)"
            }
          ]
        },
        "GetWatchlists": {
          "description": "Gets all watchlists for the user account.",
          "target": "watchlists",
          "uriTemplate": "/",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ListWatchlistResponseDTO"
          },
          "group": "Watchlist",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": []
        },
        "SaveWatchlist": {
          "description": "Save watchlist.",
          "target": "watchlist",
          "uriTemplate": "/Save",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiSaveWatchlistResponseDTO"
          },
          "group": "Watchlist",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.ApiSaveWatchlistRequestDTO",
              "name": "apiSaveWatchlistRequestDto",
              "description": "The watchlist to save."
            }
          ]
        },
        "DeleteWatchlist": {
          "description": "Delete a watchlist.",
          "target": "watchlist",
          "uriTemplate": "/delete",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "POST",
          "envelope": "JSON",
          "returns": {
            "$ref": "#.ApiDeleteWatchlistResponseDTO"
          },
          "group": "Watchlist",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "$ref": "#.ApiDeleteWatchlistRequestDTO",
              "name": "deleteWatchlistRequestDto",
              "description": "The watchlist to delete."
            }
          ]
        },
        "GenerateException": {
          "description": "Raises an error condition when an unexpected or uncontrolled event occurs.",
          "target": "errors",
          "uriTemplate": "?errorCode={errorCode}",
          "contentType": "application/json",
          "responseContentType": "application/json",
          "transport": "GET",
          "envelope": "URL",
          "returns": {
            "$ref": "#.ApiErrorResponseDTO"
          },
          "group": "Exception Handling",
          "cacheDuration": 0,
          "throttleScope": "data",
          "parameters": [
            {
              "type": "integer",
              "name": "errorCode",
              "description": "The error code for the condition encountered.",
              "demoValue": 4000
            }
          ]
        }
      }
    },
    "streaming": {
      "target": "",
      "services": {
        "NewsHeadlines": {
          "description": "Stream of current news headlines.  Try NEWS.HEADLINES.UK for a mock stream",
          "target": "CITYINDEXSTREAMING",
          "channel": "NEWS.HEADLINES.{category}",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "NewsDTO"
          },
          "group": "Streaming API",
          "parameters": [
            {
              "type": "string",
              "name": "category",
              "description": "A news category",
              "minLength": 1,
              "maxLength": 100,
              "demoValue": "UK"
            }
          ]
        },
        "Prices": {
          "description": "Stream of current prices. Try PRICES.PRICE.154297 (GBP/USD (per 0.0001) CFD) which prices Mon - Fri 24hrs",
          "target": "CITYINDEXSTREAMING",
          "channel": "PRICES.PRICE.{marketIds}",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "PriceDTO"
          },
          "group": "Streaming API",
          "parameters": [
            {
              "type": "array",
              "items": [
                {
                  "type": "integer"
                }
              ],
              "name": "marketIds",
              "description": "The marketIds",
              "demoValue": "[\"71442\", \"71443\"]"
            }
          ]
        },
        "DefaultPrices": {
          "description": "Stream of default prices for the specified account operator.  This stream does not require authentication, so can be used on a public website.  NB:  This stream returns prices for a group of markets, so check the MarketId & Name field when displaying.",
          "endpoint": "CITYINDEXSTREAMINGDEFAULTPRICES",
          "channel": "{AccountOperatorId}",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "#.PriceDTO"
          },
          "group": "Streaming API",
          "parameters": [
            {
              "type": "string",
              "name": "AccountOperatorId",
              "description": "The account operator id whose default market prices are required.  Generally you want to hardcode this depending on the brand you are using.  See http://faq.labs.cityindex.com/questions/what-are-the-list-of-accountoperatorids",
              "demoValue": "3347"
            }
          ]
        },
        "ClientAccountMargin": {
          "description": "Stream of clients current margin",
          "target": "STREAMINGCLIENTACCOUNT",
          "channel": "CLIENTACCOUNTMARGIN.ALL",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "#.ClientAccountMarginDTO"
          },
          "group": "Streaming API"
        },
        "TradeMargin": {
          "description": "Stream of trade margin",
          "target": "STREAMINGCLIENTACCOUNT",
          "channel": "TRADEMARGIN.All",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "#.TradeMarginDTO"
          },
          "group": "Streaming API"
        },
        "Orders": {
          "description": "Stream of orders",
          "target": "STREAMINGCLIENTACCOUNT",
          "channel": "ORDERS.All",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "#.OrderDTO"
          },
          "group": "Streaming API"
        },
        "Quotes": {
          "description": "Stream of quotes",
          "target": "STREAMINGTRADINGACCOUNT",
          "channel": "QUOTE.ALL",
          "transport": "HTTP",
          "protocol": "lightstreamer-4",
          "returns": {
            "$ref": "#.QuoteDTO"
          },
          "group": "Streaming API"
        }
      }
    }
  }
}