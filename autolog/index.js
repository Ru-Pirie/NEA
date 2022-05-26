const { google } = require("googleapis");
const fetch = require('node-fetch');
const fs = require('fs');
require('dotenv').config();

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

    const bodyParser = require('body-parser')
    app.use(bodyParser.json())

    app.post(`/${process.env.SPECIAL_URL}`, (req, res) => {
      updateTitleBit(googleSheetsInstance, authClientObject, sheetID)    
      addCommit(googleSheetsInstance, authClientObject, sheetID, req.body)
      console.log("RECIEVED")
    })
    
    app.get('/', async (req, res) => {
        const rawText = fs.readFileSync('./page.html', 'utf-8')
        const commitRes=  await fetch('https://api.github.com/repos/Ru-Pirie/NEA/commits', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${process.env.GITHUB_TOKEN}`
            }
        })
        let data = await commitRes.json()
        data = data.slice(0, 5);

        const progress = await fetch('https://raw.githubusercontent.com/Ru-Pirie/NEA/main/progress.json', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${process.env.GITHUB_TOKEN}`
            }
        })
        let progressData = await progress.json()
        
        console.log(progressData)

        let text = rawText.replace('{{PERCENTAGE_WRITEUP}}', progressData.writeup).replace('{{PERCENTAGE_WRITEUP}}', progressData.writeup)
        text = text.replace('{{PERCENTAGE_PROTOTYPE}}', progressData.prototype).replace('{{PERCENTAGE_PROTOTYPE}}', progressData.prototype)
        text = text.replace('{{PERCENTAGE_FINAL}}', progressData.final).replace('{{PERCENTAGE_FINAL}}', progressData.final)

        const total = (parseFloat(progressData.writeup) + parseFloat(progressData.prototype) + parseFloat(progressData.final)) / 3

        text = text.replace('{{PERCENTAGE_OVERALL}}', `${total}%` ).replace('{{PERCENTAGE_OVERALL}}', `${Math.round(total)}%` )

        text = text.replace('{{COMMIT_DATE_A}}',  new Date(data[0].commit.committer.date).toLocaleString()).replace('{{COMMIT_ID_A}}', data[0].sha.substring(0, 7)).replace('{{COMMIT_TEXT_A}}', `${data[0].commit.message}<br><br><a href="${data[0].html_url}">${data[0].html_url}</a>`)
        text = text.replace('{{COMMIT_DATE_B}}', new Date(data[1].commit.committer.date).toLocaleString()).replace('{{COMMIT_ID_B}}', data[1].sha.substring(0, 7)).replace('{{COMMIT_TEXT_B}}', `${data[1].commit.message}<br><br><a href="${data[1].html_url}">${data[1].html_url}</a>`)
        text = text.replace('{{COMMIT_DATE_C}}', new Date(data[2].commit.committer.date).toLocaleString()).replace('{{COMMIT_ID_C}}', data[2].sha.substring(0, 7)).replace('{{COMMIT_TEXT_C}}', `${data[2].commit.message}<br><br><a href="${data[2].html_url}">${data[2].html_url}</a>`)
        text = text.replace('{{COMMIT_DATE_D}}', new Date(data[3].commit.committer.date).toLocaleString()).replace('{{COMMIT_ID_D}}', data[3].sha.substring(0, 7)).replace('{{COMMIT_TEXT_D}}', `${data[3].commit.message}<br><br><a href="${data[3].html_url}">${data[3].html_url}</a>`)
        text = text.replace('{{COMMIT_DATE_E}}', new Date(data[4].commit.committer.date).toLocaleString()).replace('{{COMMIT_ID_E}}', data[4].sha.substring(0, 7)).replace('{{COMMIT_TEXT_E}}', `${data[4].commit.message}<br><br><a href="${data[4].html_url}">${data[4].html_url}</a>`)

        res.end(text);
    })

    app.all('*', (req, res) => { res.redirect("https://github.com/ru-pirie/NEA") })

    app.listen(process.env.PORT, () => {
      console.log(`READY: ${process.env.PORT}`)
    })

    
}

async function updateTitleBit(googleSheetsInstance, authClientObject, sheetID) {
    const res = await fetch('https://api.github.com/repos/ru-pirie/NEA/git/refs/heads/main', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${process.env.GITHUB_TOKEN}`
        }
    })
    const data = await res.json();

    if (data.message) return console.log('rate limited');
    const lastCommitURL = data.object.url;

    const commitRes = await fetch(`${lastCommitURL}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${process.env.GITHUB_TOKEN}`
        }
    })
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
