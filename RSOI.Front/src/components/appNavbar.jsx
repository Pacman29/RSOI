import React from "react";
import PropTypes from "prop-types";
import {Icon, NavLeft, NavRight, NavTitle, Navbar, Panel, View, Page, Block, Link, Button} from "framework7-react";
import {inject, observer} from "mobx-react";
import {action} from "mobx";
import AuthStore from "../stores/AuthStore";

@inject("authStore")
@observer
export default class AppNavbar extends React.Component {
    static propTypes = {
        authStore: PropTypes.instanceOf(AuthStore),
        back: PropTypes.bool,
    };


    render(){
        return (
            <React.Fragment>
                <Navbar>
                    {this.props.back ? (<NavLeft backLink="Back"/>) : <NavLeft/>}
                    <NavTitle>
                        Recognize Service
                    </NavTitle>
                    <NavRight>
                        <Link raised panelOpen="left" iconF7="person"/>
                    </NavRight>
                </Navbar>
            </React.Fragment>
        );
    }
}