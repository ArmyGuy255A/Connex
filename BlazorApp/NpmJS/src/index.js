// Importing only specific components from @tabler/core
import { Tooltip } from '@tabler/core';

import './index.scss';

window.initializeTooltips = () => {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
};
