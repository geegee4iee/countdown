import * as Actions from './AppUsageReportActions';
import { mapReducersToActions } from '../../utils/ReducerUtils';

const initialState = {
    appUsageRecords: []
}

const reducers = {
    [Actions.ActionTypes.RECEIVE_LAST_WEEK_APP_USAGE_REPORT]: (state, actionPayload) => {
        return {
            appUsageRecords: actionPayload.appUsageRecords
        }
    }
}

export default mapReducersToActions(reducers, initialState);