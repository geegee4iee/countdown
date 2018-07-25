import React from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

export default props => (
  <Navbar inverse fixedTop fluid collapseOnSelect>
    <Navbar.Header>
      <Navbar.Brand>
        <Link to={'/'}>Coundown.Spa.ReactJs</Link>
      </Navbar.Brand>
      <Navbar.Toggle />
    </Navbar.Header>
    <Navbar.Collapse>
      <Nav>
        <LinkContainer to={'/'} exact>
          <NavItem>
            <Glyphicon glyph='home' /> Home
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/appusage'}>
          <NavItem>
            <Glyphicon glyph='th-list' /> App Usage
          </NavItem>
        </LinkContainer>
        <LinkContainer to={'/karma'}>
          <NavItem>
            <Glyphicon glyph='th-list'/> Karma
          </NavItem>
        </LinkContainer>
      </Nav>
    </Navbar.Collapse>
  </Navbar>
);
