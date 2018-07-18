import keyMirror from 'keymirror';
import moment from 'moment';

export const ActionTypes = keyMirror({
    RECEIVE_APP_USAGE_KARMA_RESULTS: null
});

function receiveAppUsageKarmaResults(json) {
    return {
        type: ActionTypes.RECEIVE_APP_USAGE_KARMA_RESULTS,
        payload: {
            karmaResults: json
        }
    }
}

const requestLastWeekAppUsageRecords = (date) => async (dispatch) => {
    const dateStr = moment(date).format('YYYY-MM-DD');
    const reponse = await fetch(`/api/PredictKarma/GetPredictedForDate/${dateStr}`);
    const karmaResults = await reponse.json();

    dispatch(receiveAppUsageKarmaResults(karmaResults));
}

export const Actions = {
    requestLastWeekAppUsageRecords,
}

