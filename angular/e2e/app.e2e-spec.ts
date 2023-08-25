import { HRMv2TemplatePage } from './app.po';

describe('HRMv2 App', function() {
  let page: HRMv2TemplatePage;

  beforeEach(() => {
    page = new HRMv2TemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
