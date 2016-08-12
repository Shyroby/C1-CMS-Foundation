import React, { PropTypes } from 'react';
import Icon from './Icon.js';

const ActionButton = ({ label, action, icon }) => (
	<button onClick={() => action()}>
		{icon ? <Icon id={icon}/> : null}
		<span>{label}</span>
	</button>
);


ActionButton.propTypes = {
	label: PropTypes.string.isRequired,
	action: PropTypes.func.isRequired,
	icon: PropTypes.string
};

export default ActionButton;