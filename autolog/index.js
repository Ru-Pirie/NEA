const { google } = require("googleapis");
const fetch = require('node-fetch');
const fs = require('fs');

async function main() {
    const auth = new google.auth.GoogleAuth({
        //the key file
        keyFile: "creds.json", 
        //url to spreadsheets API
        scopes: "https://www.googleapis.com/auth/spreadsheets", 
    });
    
    const authClientObject = await auth.getClient();
    
    const sheetID = '1M0x6IfIWKX9heKnwUo0QLy9aNsVWG9BsSVViUWelrtU';
    const googleSheetsInstance = google.sheets({ version: "v4", auth: authClientObject });

    updateTitleBit(googleSheetsInstance, authClientObject, sheetID)       
    addCommit(googleSheetsInstance, authClientObject, sheetID, JSON.parse(fs.readFileSync('dummyPayload.json', 'UTF-8')), true)
  
    const express = require('express')
    const app = express()
    const port = 80

    const bodyParser = require('body-parser')
    app.use(bodyParser.json())

    app.post(CHANGEME, (req, res) => {
      updateTitleBit(googleSheetsInstance, authClientObject, sheetID)    
      addCommit(googleSheetsInstance, authClientObject, sheetID, req.body)
      console.log("RECIEVED")
    })
    
    app.listen(port, () => {
      console.log(`READY: ${port}`)
    })

    
}

async function updateTitleBit(googleSheetsInstance, authClientObject, sheetID) {
    const res = await fetch('https://api.github.com/repos/ru-pirie/NEA/git/refs/heads/main')
    const data = await res.json();
    const lastCommitURL = data.object.url;

    const commitRes = await fetch(`${lastCommitURL}`)
    const commitData = await commitRes.json();
    const lastCommitMSG = commitData.message
    const lastCommitTime = new Date(commitData.committer.date).toLocaleString();
    
    await googleSheetsInstance.spreadsheets.values.update({
        auth: authClientObject,
        spreadsheetId: sheetID,
        range: "NEA!C4",
        valueInputOption: "USER_ENTERED",
        resource: {
            values: [[lastCommitTime]]
        },
    })
      await googleSheetsInstance.spreadsheets.values.update({
        auth: authClientObject,
        spreadsheetId: sheetID,
        range: "NEA!C5",
        valueInputOption: "USER_ENTERED",
        resource: {
            values: [[`${lastCommitMSG} (${commitData.sha.substring(0, 7)})`]]
        },
    })
}

async function addCommit(instance, auth, id, payload, val) {
    const commit = payload.commits[0]
    
    await instance.spreadsheets.values.append({
        auth: auth,
        spreadsheetId: id,
        range: "NEA!A:I",
        valueInputOption: "USER_ENTERED",
        resource: {
            values: [[
              commit.id.substring(0, 7),
              val ? new Date().toLocaleString() : new Date(commit.timestamp).toLocaleString(),
              `${commit.committer.name} (${commit.committer.email})`,
              val ? `https://github.com/Ru-Pirie/NEA` : `https://github.com/Ru-Pirie/NEA/commit/${commit.id.substring(0, 7)}`,
              val ? "" : commit.message,
              commit.added.length !== 0 ? commit.added.join('\n') : "None",
              commit.removed.length !== 0 ? commit.removed.join('\n') : "None",
              commit.modified.length !== 0 ? commit.modified.join('\n') : "None",
              val ? "" : "Teacher:\nStudent:"
            ]]
        },
    })
}

main()
