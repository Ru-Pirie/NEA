const { google } = require("googleapis");
const fetch = require('node-fetch');

async function main() {
    const res = await fetch('https://api.github.com/repos/ru-pirie/NEA/git/refs/heads/main')

    const auth = new google.auth.GoogleAuth({
        //the key file
        keyFile: "creds.json", 
        //url to spreadsheets API
        scopes: "https://www.googleapis.com/auth/spreadsheets", 
    });
    
    const authClientObject = await auth.getClient();
    
    const sheetID = '1M0x6IfIWKX9heKnwUo0QLy9aNsVWG9BsSVViUWelrtU';
    const googleSheetsInstance = google.sheets({ version: "v4", auth: authClientObject });

    await googleSheetsInstance.spreadsheets.values.update({
        auth: authClientObject,
        spreadsheetId: sheetID,
        range: "NEA!C4",
        valueInputOption: "USER_ENTERED",
        resource: {
            values: [[new Date().toLocaleString()]]
        },
    })

    await googleSheetsInstance.spreadsheets.values.append({
        auth: authClientObject,
        spreadsheetId: sheetID,
        range: "NEA!A:D",
        valueInputOption: "USER_ENTERED",
        resource: {
            values: [["testid", "this is a test message", "> ", "https://google.com"]]
        },
    })
}

// 
//



main()