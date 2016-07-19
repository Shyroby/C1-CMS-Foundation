import React, { PropTypes } from 'react';
import ActionButton from './ActionButton.js';

const typeClassName = {
	document: " document-toolbar"
}

const Toolbar = ({ type, actions, buttons, getState }) => (
	<div className={"toolbar" + (typeClassName[type] || '')}>
		{Object.keys(buttons).map(
			name => <ActionButton key={name} action={actions[name]} {...buttons[name]}/>
		)}
	</div>
);

Toolbar.propTypes = {
	type: PropTypes.string,
	actions: PropTypes.object.isRequired,
	buttons: PropTypes.object.isRequired
};

export default Toolbar;
