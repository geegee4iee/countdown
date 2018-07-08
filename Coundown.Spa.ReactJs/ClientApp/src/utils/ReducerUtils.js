export const mapReducersToActions = (reducers, initialState) => (state, action) => {
    state = state || initialState;
    if (!reducers[action.type]) return state;

    return {
        ...state,
        ...reducers[action.type](state, action.payload)
    }
}