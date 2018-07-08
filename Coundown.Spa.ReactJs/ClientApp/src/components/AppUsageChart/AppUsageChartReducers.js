import * as Actions from './AppUsageChartActions';
import { mapReducersToActions } from '../../utils/ReducerUtils';

const initialState = {
    processInfoCollection: []
}

const reducers = {
    [Actions.ActionTypes.RECEIVE_PROCESS_INFO_COLLECTION]: (state, actionPayload) => {
        return {
            processInfoCollection: actionPayload.processInfoCollection
        }
    }
}

export default mapReducersToActions(reducers, initialState);