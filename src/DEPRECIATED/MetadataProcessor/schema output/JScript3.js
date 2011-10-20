var schema = {
    "properties": {
        "PriceTickDTO": {
            "id": "PriceTickDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "TickDate": {
                    "type": "string"
                },
                "Price": {
                    "type": "number"
                }
            }
        },
        "OrderDTO": {
            "id": "OrderDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "OrderId": {
                    "type": "integer"
                },
                "CurrencyId": {
                    "type": "integer"
                },
                "DirectionId": {
                    "type": "integer"
                },
                "MarketId": {
                    "type": "integer"
                },
                "Quantity": {
                    "type": "number"
                },
                "StatusId": {
                    "type": "integer"
                }
            }
        },
        "StopLimitOrderDTO": {
            "id": "StopLimitOrderDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "TriggerPrice": {
                    "type": "number"
                },
                "StopPrice": {
                    "type": "number"
                },
                "LimitPrice": {
                    "type": "number"
                }
            },
            "extends": {
                "$ref": "#.properties.OrderDTO"
            }
        },
        "NewsDTO": {
            "id": "NewsDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "StoryId": {
                    "type": "integer"
                },
                "Headline": {
                    "type": "string"
                },
                "PublishDate": {
                    "type": "string"
                }
            }
        },
        "NewsDetailDTO": {
            "id": "NewsDetailDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Story": {
                    "type": "string"
                }
            },
            "extends": {
                "$ref": "#.properties.NewsDTO"
            }
        },
        "MarketDTO": {
            "id": "MarketDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "MarketId": {
                    "type": "integer"
                },
                "Name": {
                    "type": "string"
                }
            }
        },
        "PriceBarDTO": {
            "id": "PriceBarDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "BarDate": {
                    "type": "string"
                },
                "Open": {
                    "type": "number"
                },
                "High": {
                    "type": "number"
                },
                "Low": {
                    "type": "number"
                },
                "Close": {
                    "type": "number"
                }
            }
        },
        "AccountDetailsDTO": {
            "id": "AccountDetailsDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "AccountId": {
                    "type": "integer"
                },
                "AccountName": {
                    "type": "string"
                }
            }
        },
        "GetNewsDetailResponseDTO": {
            "id": "GetNewsDetailResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "NewsDetail": {
                    "$ref": "#.properties.NewsDetailDTO"
                }
            }
        },
        "NewTradeOrderRequestDTO": {
            "id": "NewTradeOrderRequestDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "MarketId": {
                    "type": "integer"
                },
                "Direction": {
                    "type": "integer"
                },
                "Quantity": {
                    "type": "number"
                },
                "BidPrice": {
                    "type": "number"
                },
                "OfferPrice": {
                    "type": "number"
                },
                "AuditId": {
                    "type": "string"
                }
            }
        },
        "LogOnRequestDTO": {
            "id": "LogOnRequestDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "UserName": {
                    "type": "string"
                },
                "Password": {
                    "type": "string"
                }
            }
        },
        "AccountLogOnRequestDTO": {
            "id": "AccountLogOnRequestDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "ClientAccountId": {
                    "type": "integer"
                },
                "TradingAccountId": {
                    "type": "integer"
                }
            },
            "extends": {
                "$ref": "#.properties.LogOnRequestDTO"
            }
        },
        "NewTradeOrderResponseDTO": {
            "id": "NewTradeOrderResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Status": {
                    "type": "integer"
                },
                "StatusReason": {
                    "type": "integer"
                },
                "OrderId": {
                    "type": "integer"
                }
            }
        },
        "CreateSessionResponseDTO": {
            "id": "CreateSessionResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Session": {
                    "type": "string"
                }
            }
        },
        "ErrorResponseDTO": {
            "id": "ErrorResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "ErrorMessage": {
                    "type": "string"
                },
                "ErrorCode": {
                    "$ref": "#.properties.ErrorCode"
                }
            }
        },
        "GetPriceTickResponseDTO": {
            "id": "GetPriceTickResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "PriceTicks": {
                    "type": "array",
                    "items": {
                        "$ref": "#.properties.PriceTickDTO"
                    }
                }
            }
        },
        "ListSpreadMarketsResponseDTO": {
            "id": "ListSpreadMarketsResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Markets": {
                    "type": "array",
                    "items": {
                        "$ref": "#.properties.MarketDTO"
                    }
                }
            }
        },
        "ErrorCode": {
            "type": "integer",
            "enum": [
                403,
                500,
                4000,
                4001,
                4002,
                4003,
                4004
            ],
            "options": [
                {
                    "value": 403,
                    "label": "Forbidden"
                },
                {
                    "value": 500,
                    "label": "InternalServerError"
                },
                {
                    "value": 4000,
                    "label": "InvalidParameterType"
                },
                {
                    "value": 4001,
                    "label": "ParameterMissing"
                },
                {
                    "value": 4002,
                    "label": "InvalidParameterValue"
                },
                {
                    "value": 4003,
                    "label": "InvalidJsonRequest"
                },
                {
                    "value": 4004,
                    "label": "InvalidJsonRequestCaseFormat"
                }
            ]
        },
        "ListCfdMarketsResponseDTO": {
            "id": "ListCfdMarketsResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Markets": {
                    "type": "array",
                    "items": {
                        "$ref": "#.properties.MarketDTO"
                    }
                }
            }
        },
        "SystemStatusRequestDTO": {
            "id": "SystemStatusRequestDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "TestDepth": {
                    "type": "string"
                }
            }
        },
        "SessionDeletionRequestDTO": {
            "id": "SessionDeletionRequestDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "UserName": {
                    "type": "string"
                },
                "Session": {
                    "type": "string"
                }
            }
        },
        "G2SessionValidationResponseDTO": {
            "id": "G2SessionValidationResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "ClientAccountIds": {
                    "type": "array",
                    "items": {
                        "type": "integer"
                    }
                },
                "TradingAccountIds": {
                    "type": "array",
                    "items": {
                        "type": "integer"
                    }
                },
                "IsValid": {
                    "type": "boolean"
                }
            }
        },
        "SystemStatusDTO": {
            "id": "SystemStatusDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "StatusMessage": {
                    "type": "string"
                }
            }
        },
        "GetPriceBarResponseDTO": {
            "id": "GetPriceBarResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "PriceBars": {
                    "type": "array",
                    "items": {
                        "$ref": "#.properties.PriceBarDTO"
                    }
                },
                "PartialPriceBar": {
                    "$ref": "#.properties.PriceBarDTO"
                }
            }
        },
        "ListNewsHeadlinesResponseDTO": {
            "id": "ListNewsHeadlinesResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Headlines": {
                    "type": "array",
                    "items": {
                        "$ref": "#.properties.NewsDTO"
                    }
                }
            }
        },
        "SessionDeletionResponseDTO": {
            "id": "SessionDeletionResponseDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "LoggedOut": {
                    "type": "boolean"
                }
            }
        },
        "SessionValidationRequestDTO": {
            "id": "SessionValidationRequestDTO",
            "description": "foo desc",
            "type": "object",
            "properties": {
                "Session": {
                    "type": "string"
                }
            }
        }
    }
}