import React, { Component } from 'react';
import ReactTable from 'react-table';
import 'react-table/react-table.css';
import _ from 'lodash';

export default class SubPieDetailTable extends Component {
    render() {
        const data = this.props.data || [];

        const columns = [{
            Header: 'Title',
            accessor: 'windowTitle'
        }, 
        {
            Header: 'Total',
            accessor: 'seconds',
            Cell: props => <span className='number'>{props.value > 60 ? `${_.round(props.value/60)} minutes` : `${props.value} seconds`}</span>
        }]

        return (
        <ReactTable
            data={data}
            columns={columns}/>
        );
    }
}