import React, { Component } from 'react';
import { formatDate, parseDate } from 'react-day-picker/moment';
import DayPickerInput from 'react-day-picker/DayPickerInput';
import 'moment/locale/it';

import './DayPickerInput.css';

export default class DayPickerInputWrapper extends Component {
    render() {
        return (
            <DayPickerInput
                formatDate={formatDate}
                parseDate={parseDate}
                format={'YYYY MM DD'}
                placeholder={`${formatDate(new Date(), 'YYYY MM DD')}`} 
                {...this.props}/>
        );
    }
}