import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

// Local
import { Actions } from './AppUsageReportActions';

class AppUsageReport extends Component {
    componentWillMount() {
        this.props.requestLastWeekAppUsageRecords();
    }

    componentDidUpdate() {
        
    }

    render() {
        return (
            <React.Fragment>
                Report
            </React.Fragment>
        )
    }
}

export default connect(
    state => state.appUsageReport,
    dispatch => bindActionCreators(Actions, dispatch)
) (AppUsageReport)