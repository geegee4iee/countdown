import keyMirror from 'keymirror';
import moment from 'moment';

export const ActionTypes = keyMirror({
    RECEIVE_PROCESS_INFO_COLLECTION: null
});

function receiveProcessInfoCollection(json) {
    return {
        type: ActionTypes.RECEIVE_PROCESS_INFO_COLLECTION,
        payload: {
            processInfoCollection: json
        }
    }
}

const requestTodayProcessInfoCollection = () => async (dispatch) => {
    const reponse = await fetch(`/api/AppUsageRecord/today`);
    const processInfoCollection = await reponse.json();

    dispatch(receiveProcessInfoCollection(processInfoCollection));
}

const requestProcessInfoCollectionForDate = (date) => async (dispatch) => {
    const dateStr = moment(date).format('YYYY-MM-DD');

    const reponse = await fetch(`/api/AppUsageRecord/${dateStr}`);
    const processInfoCollection = await reponse.json();

    dispatch(receiveProcessInfoCollection(processInfoCollection));
}

export const Actions = {
    receiveProcessInfoCollection,
    requestTodayProcessInfoCollection,
    requestProcessInfoCollectionForDate
}

