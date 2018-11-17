import React from "react";
import PropTypes from "prop-types";
//import {ProgressCircular} from "react-onsenui";
import {observer} from "mobx-react";
import {Preloader} from "framework7-react";

const styles = {
    progressStyle: {
        width: '90px',
        height: '90px',
    },
    loaderStyle: {
        display: "flex",
        alignItems: 'center',
        justifyContent: 'center',
        height: '100%'
    }
};

@observer
export default class Loader extends React.Component{
    static propTypes = {
        isLoading: PropTypes.bool
    };

    constructor(props){
        super(props)
    }

    render(){
        return this.props.isLoading ? (
                    <div style={styles.loaderStyle}>
                        <Preloader style={styles.progressStyle} />
                    </div>
                ) : this.props.children;
    }
}