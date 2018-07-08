import keyMirror from 'keymirror';

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
    const reponse = await fetch(`https://localhost:5001/api/AppUsageRecord/today`);
    const processInfoCollection = await reponse.json();

    dispatch(receiveProcessInfoCollection(processInfoCollection));
}

export const Actions = {
    receiveProcessInfoCollection,
    requestTodayProcessInfoCollection
}

