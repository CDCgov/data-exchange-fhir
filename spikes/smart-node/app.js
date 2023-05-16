( async function start() {

    const dotenv = require('dotenv').config()
    const fhirclient = require('fhirclient')

    console.log('starting smart auth node app..')
    console.log('loading environment variables..')

    const serverUrl = process.env.SERVER_URL
    const clientId = process.env.CLIENT_ID
    const redirectUri = process.env.REDIRECT_URI

    console.log(`serverUrl: ${serverUrl}`)
    console.log(`clientId: ${clientId}`)
    console.log(`redirectUri: ${redirectUri}`) 


    // Create an instance of the SMART client
    const client = fhirclient({
        serviceUrl: serverUrl,
        clientId: clientId,
        redirectUri: redirectUri,
        scope: "launch openid fhirUser patient/*.read",
    })

    // Launch the SMART client authorization flow
    client.authorize({
        clientId: client.clientId,
        scope: client.scope,
        redirectUri: client.redirectUri,
        // launch: launchUri,
      }).then(tokenResponse => {
        // Access token obtained successfully
        console.log('Access token:', tokenResponse.access_token)

      }).catch(error => {
        // Handle errors
        console.error('An error occurred:', error)
      })


})()