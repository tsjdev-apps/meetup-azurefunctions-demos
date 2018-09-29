# Serverless Computing - just a hype

## An introduction to Azure Functions

### Introduction

During the Meetup *Serverless Computing - just a hype? An introduction to Azure Functions* we gave a brave introduction into Azure Functions. This repository contains most of our samples.

### Preperation

To be able to run the sample locally on your machine you need to clone the repo and add a file `local.settings.json` in the root path of the project.

Than you need to provide data like shown in the following structure if your are using v1 of the runtime:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "statusReportFromMail": "",
    "statusReportToMail": "",
    "statusReportMailPassword": "",
    "statusReportHost": "",
    "statusReportPort": "",
    "miteTenant": "",
    "miteApiKey": "",
    "twitterConsumerKey": "",
    "twitterConsumerSecret": "",
    "twitterAccessToken": "",
    "twitterAccessTokenSecret": ""
  }
}
```

and the following json for v2 of the runtime:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "statusReportFromMail": "",
    "statusReportToMail": "",
    "statusReportMailPassword": "",
    "statusReportHost": "",
    "statusReportPort": "",
  }
}
```


The values `statusReportFromMail`, `statusReportToMail`, `statusReportMailPassword`, `statusReportHost` and `statusReportPort` are needed for the `PeriodicStatusReportFunction` to send every minute an email to a provided mail address.

The values `miteTenant` and `miteApiKey` are needed for the two functions `MiteProjectTimesStatusFunction` and `MiteWeeklyTrackedTimeFunction` to be able to access the [Mite](https://mite.yo.lk) api.

The values `twitterConsumerKey`, `twitterConsumerSecret`, `twitterAccessToken` and `twitterAccessTokenSecret` are needed for the function `MiteWeeklyTrackedTimeFunction` to be able to send a tweet via [Twitter](https://twitter.com).

### Resources

- [Azure Functions](https://azure.microsoft.com/en-us/services/functions/)
- [Cognitive Service](https://azure.microsoft.com/en-us/services/cognitive-services/)
- [Microsoft Botframework](https://dev.botframework.com/)
- [Mite](https://mite.yo.lk)
- [Twitter Credentials](https://apps.twitter.com/)

### Questions and Feedback

If you have any follow up questions, feel free to talk to us on Twitter:

- Sebastian Jensen - [@tsjdevapps](https://twitter.com/tsjdevapps)
- Robert Schlaeger - [@RobAnybody_](https://twitter.com/RobAnybody_)