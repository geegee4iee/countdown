import keyMirror from 'keymirror';

export const ActionTypes = keyMirror({
    RECEIVE_LAST_WEEK_APP_USAGE_REPORT: null
});

function receiveAppUsageRecords(json) {
    return {
        type: ActionTypes.RECEIVE_LAST_WEEK_APP_USAGE_REPORT,
        payload: {
            appUsageRecords: json
        }
    }
}

const requestLastWeekAppUsageRecords = () => async (dispatch) => {
    const reponse = await fetch(`https://localhost:5001/api/AppUsageRecord/lastweek`);
    const appUsageRecords = await reponse.json();

    dispatch(receiveAppUsageRecords(appUsageRecords));
}

export const Actions = {
    receiveAppUsageRecords,
    requestLastWeekAppUsageRecords
}

