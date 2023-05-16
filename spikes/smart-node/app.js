

( async function start() {

    const dotenv = require('dotenv').config()

    console.log('starting smart auth node app..')

    const serverUrl = process.env.SERVER_URL
    const clientId = process.env.CLIENT_ID
    const redirectUri = process.env.REDIRECT_URI

    console.log(`serverUrl: ${serverUrl}`)
    console.log(`clientId: ${clientId}`)
    console.log(`redirectUri: ${redirectUri}`) 



})()