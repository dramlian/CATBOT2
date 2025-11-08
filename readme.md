# Cat Photo Bot - Azure HTTP Function with Meta Webhook

This Azure Function project implements a Facebook Messenger bot that automatically sends cute cat photos to users who message your Facebook page. The bot uses Azure HTTP-triggered functions to handle Meta webhooks and respond with adorable cat images.

## How It Works

1. Users send messages to your Facebook page via Messenger
2. Facebook sends webhook events to your Azure Function endpoint
3. The function processes the incoming message
4. Bot responds automatically with a cute cat photo utilizing free cat api https://developers.thecatapi.com/

## Architecture

- **HTTP Trigger Function**: The azure function receives POST requests from Facebook's webhook system
- **Webhook Verification**: Validates incoming requests using Facebook's verification token
- **Message Processing**: Parses incoming message events and user data
- **Cat Photo API Integration**: Fetches random cat images from external API
- **Messenger Send API**: Sends cat photos back to users via Facebook's Graph API

## Setup Instructions

### Prerequisites

- Azure subscription with Function App deployed
- Facebook Developer Account
- Facebook Page for the bot

### Facebook Developer Setup

1. **Create Facebook App**: Visit [Facebook Developers](https://developers.facebook.com/) and create a new app
2. **Configure Webhooks**: Follow the official [Facebook Graph API Webhooks Documentation](https://developers.facebook.com/docs/graph-api/webhooks/)
3. **Set Webhook URL**: Point to your Azure Function HTTP endpoint
4. **Configure Page Access Token**: Generate token for your Facebook page
5. **Subscribe to Events**: Enable `messages` and `messaging_postbacks` events

### Azure Function Configuration

1. **Deploy Function**: Deploy this project to your Azure Function App
2. **Environment Variables**: Configure the following app settings:

   - `META_PAT`: Page access token from Facebook
   - `META_APP_SECRET`: Your webhook verification token (the secret)

3. **HTTP Endpoint**: Note your function's HTTP trigger URL for webhook configuration

## Learn More

- [Azure Functions HTTP Triggers](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook)
- [Facebook Messenger Platform](https://developers.facebook.com/docs/messenger-platform/)
- [Meta Webhooks Documentation](https://developers.facebook.com/docs/graph-api/webhooks/)
