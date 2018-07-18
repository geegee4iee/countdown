import * as Actions from './AppUsageKarmaActions';
import { mapReducersToActions } from '../../utils/ReducerUtils';

const initialState = {
    karmaResults: []
}

const reducers = {
    [Actions.ActionTypes.RECEIVE_APP_USAGE_KARMA_RESULTS]: (state, actionPayload) => {
        return {
            karmaResults: actionPayload.karmaResults
        }
    }
}

export default mapReducersToActions(reducers, initialState);