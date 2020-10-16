import { getRenderer } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/transport/renderer.js';
import { initJssCs } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/transport/setup-jss.js';initJssCs();
import { installTheme } from 'C:/Repositories/GitHubIssuePolice/.codedoc/content/theme.ts';installTheme();
import { codeSelection } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/code/selection.js';codeSelection();
import { sameLineLengthInCodes } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/code/same-line-length.js';sameLineLengthInCodes();
import { initHintBox } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/code/line-hint/index.js';initHintBox();
import { initCodeLineRef } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/code/line-ref/index.js';initCodeLineRef();
import { initSmartCopy } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/code/smart-copy.js';initSmartCopy();
import { copyHeadings } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/heading/copy-headings.js';copyHeadings();
import { contentNavHighlight } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/page/contentnav/highlight.js';contentNavHighlight();
import { loadDeferredIFrames } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/transport/deferred-iframe.js';loadDeferredIFrames();
import { smoothLoading } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/transport/smooth-loading.js';smoothLoading();
import { tocHighlight } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/page/toc/toc-highlight.js';tocHighlight();
import { postNavSearch } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/page/toc/search/post-nav/index.js';postNavSearch();
import { copyLineLinks } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/code/line-links/copy-line-link.js';copyLineLinks();
import { gatherFootnotes } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/footnote/gather-footnotes.js';gatherFootnotes();
import { ToCPrevNext } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/page/toc/prevnext/index.js';
import { CollapseControl } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/collapse/collapse-control.js';
import { GithubSearch } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/misc/github/search.js';
import { ToCToggle } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/page/toc/toggle/index.js';
import { DarkModeSwitch } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/components/darkmode/index.js';
import { ConfigTransport } from 'C:/Repositories/GitHubIssuePolice/.codedoc/node_modules/@codedoc/core/dist/es5/transport/config.js';

const components = {
  'Xe9iObdUoxxjTmEwEpNuNg==': ToCPrevNext,
  'UF0ADWAtXpt77Xnn3/6MRA==': CollapseControl,
  'wG0ZPsDewoypwFHAakVU7Q==': GithubSearch,
  '/LXrPBJFkDmTncuU6oWrTA==': ToCToggle,
  'JEe08i+lr92HI6xcDyXo4g==': DarkModeSwitch,
  'vVx1EDeks46rCWeEnus9yw==': ConfigTransport
};

const renderer = getRenderer();
const ogtransport = window.__sdh_transport;
window.__sdh_transport = function(id, hash, props) {
  if (hash in components) {
    const target = document.getElementById(id);
    renderer.render(renderer.create(components[hash], props)).after(target);
    target.remove();
  }
  else if (ogtransport) ogtransport(id, hash, props);
}
