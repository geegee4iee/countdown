import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';

import AppUsageChart from './components/AppUsageChart/AppUsageChart';
import AppUsageKarma from './components/AppUsageKarma/AppUsageKarma';

export default () => (
  <Layout>
    <Route exact path='/' component={AppUsageChart} />
    <Route path='/appusage' component={AppUsageChart} />
    <Route path='/karma' component={AppUsageKarma}/>
  </Layout>
);
