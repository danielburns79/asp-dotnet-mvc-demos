function angleRotate(p, angle, anchor)
{
    var values = angle.match(/[\-\d]+/g);
    var _sin = Math.sin(values[0] * Math.PI / 180);
    var _cos = Math.cos(values[0] * Math.PI / 180);
    var r = {
        x: (_cos * (p.x - anchor.x)) + (-_sin * (p.y - anchor.y)) + anchor.x,
        y: (_sin * (p.x - anchor.x)) + (_cos * (p.y - anchor.y)) + anchor.y
    };
    return r;
}
function matrixRotate(p, matrix, anchor)
{
    // [cos(a), sin(a), -sin(a), cos(a), 0, 0]
    var values = matrix.match(/[\-\d\.]+/g);
    var r = {
        x: (values[0] * (p.x - anchor.x)) + (values[2] * (p.y - anchor.y)) + anchor.x,
        y: (values[1] * (p.x - anchor.x)) + (values[3] * (p.y - anchor.y)) + anchor.y
        };
    return r;
}
function transformRotate(p, transform, anchor)
{
    if (transform.search('matrix') >= 0)
    {
        return matrixRotate(p, transform, anchor);
    }
    else if (transform.search('rotate') >= 0)
    {
        return angleRotate(p, transform, anchor);
    }
    else
    {
        alert("not a valid transform rotate " + transform);
    }
}

jQuery.fn.updateCounter = function(counter)
{
    diagramElementCounter = counter;
    return this;
}

function distanceToPoint(v, w)
{
    return Math.sqrt(Math.pow(v.x - w.x, 2) + Math.pow(v.y - w.y, 2));
}
function distanceToLineSegment(p, v, w)
{
    var l2 = distanceToPoint(v, w);
    if (l2 == 0) return distanceToPoint(p, v);
    var t = ((p.x - v.x) * (w.x - v.x) + (p.y - v.y) * (w.y - v.y)) / Math.pow(l2, 2);
    t = Math.max(0, Math.min(1, t));
    return distanceToPoint(p, {x: v.x + t * (w.x - v.x), y: v.y + t * (w.y - v.y)});
}
jQuery.fn.closestToOffset = function(offset) {
    var closestElement = null,
        elementOffset,
        x = offset.left,
        y = offset.top,
        p1, p2, p3, p4,
        distance, minDistance;
    //console.log("closestToOffset: x=" + x + ", y=" + y);
    this.each(function() {
        var currentElement = $(this);
        elementOffset = currentElement.offset();
        p1 = { x: elementOffset.left, y: elementOffset.top };
        p2 = { x: elementOffset.left + currentElement.width(), y: p1.y };
        p3 = { x: p1.x, y: elementOffset.top + currentElement.height() };
        p4 = { x: p2.x, y: p3.y };
        var transform = currentElement.css('transform');
        if (transform && transform.search('none') < 0)
        {
            p2 = transformRotate(p2, transform, p1);
            p4 = transformRotate(p4, transform, p3);
        }
        else
        {
            if (x >= p1.x &&
                x <= p2.x &&
                y >= p1.y &&
                y <= p2.y)
            {
                closestElement = currentElement;
                //console.log("closestToOffset: offset is within element " + currentElement.attr('id'));
                return false;
            }
        }
        var distance = distanceToLineSegment({x: x, y: y}, p1, p4);
        if (minDistance === undefined || distance < minDistance)
        {
            minDistance = distance;
            closestElement = currentElement;
            //console.log("closestToOffset: offset is close to element=" + currentElement.attr('id') + ", distance=" + distance);
        }
    });
    return closestElement;
};

jQuery.fn.drawLine = function(x1,y1, x2,y2)
{
    var length = Math.sqrt((x1-x2)*(x1-x2) + (y1-y2)*(y1-y2));
    var angle  = Math.atan2(y2 - y1, x2 - x1) * 180 / Math.PI;
    var transform = 'rotate('+angle+'deg)';
    this.css({'transform': transform});
    this.width(length);
    this.css({'left': x1, 'top': y1});
    return this;
}

// TODO - refactor creating/adding/drawing elements into classes

function maxModifiersHeight(elements)
{
    var height = 0;
    elements.each(function() {
        var element = $(this);
        // TODO - is this the right place for conj?
        if (element.hasClass('diagram-element-conj'))
        {
            //console.log("maxModifiersHeight: element is diagram-element-conj");
            height = Math.max(height, getBottomHeight(element.children('.diagram-clause')) + getTopHeight(element.children('.diagram-clause')));
        }
        else
        {
            //console.log("maxModifiersHeight: element="+element.attr('class')+" getElementHeight");
            height = Math.max(height, element.getElementHeight(false));
            //console.log("maxModifiersHeight: element="+element.attr('class')+" height="+height);
        }
    });
    //console.log("maxModifiersHeight: height="+height);
    return height;
}
jQuery.fn.getElementHeight = function(compound)
{
    if (compound)
    {
        //console.log("getElementHeight: compound element="+this.attr('class')+" maxHeight + maxElementHeight");
        return maxHeight(this.children('.diagram-part-line')) + maxElementHeight(this.children('.diagram-element'));
    }
    else
    {
        //console.log("getElementHeight: !compound element="+this.attr('class')+" maxHeight || maxElementHeight");
        return Math.max(maxHeight(this.children('.diagram-part-line')), maxElementHeight(this.children('.diagram-element')));
    }
}
function maxElementHeight(elements)
{
    var height = 0;
    elements.each(function() {
        var element = $(this);
        //console.log("maxElementHeight: element="+element.attr('class')+" getElementHeight");
        height = Math.max(height, element.getElementHeight(true));
        //console.log("maxElementHeight: element="+element.attr('class')+" height="+height);
    });
    //console.log("maxElementHeight: height="+height);
    return height;
}
function maxHeight(elements)
{
    var height = 0;
    elements.each(function() {
        var element = $(this);
        if (element.hasClass('diagram-part-line-height-skip'))
        {
            return true; // continue elements.each
        }
        height = Math.max(height, element.getHeight());
        //console.log("maxHeight: element="+element.attr('class')+" height="+height);
    });
    //console.log("maxHeight: height="+height);
    return height;
}
jQuery.fn.getHeight = function()
{
    elementOffset = this.offset();
    p1 = { x: elementOffset.left, y: elementOffset.top };
    p2 = { x: elementOffset.left + this.width(), y: p1.y };
    var transform = this.css('transform');
    if (transform && transform.search('none') < 0)
    {
        p2 = transformRotate(p2, transform, p1);
    }
    //console.log("getHeight: element="+this.attr('class')+" height="+(p2.y-p1.y));
    return p2.y - p1.y;
}
function getBottomHeight(elements)
{
    //console.log("getBottomHeight++");
    var total = 0;
    elements.each(function() {
        var element = $(this);
        //console.log("getBottomHeight element="+element.attr('class')+" maxModifiersHeight");
        total += maxModifiersHeight(element.children('.diagram-element'));
        //console.log("getBottomHeight element="+element.attr('class')+" total="+total);
    });
    //console.log("getBottomHeight total="+total);
    return total;
}
function maxTopHeight(elements)
{
    var height = 0;
    elements.each(function() {
        var element = $(this);
        //console.log("maxTopHeight: element="+element.attr('class'));
        height = Math.max(height, maxTopHeight(element.children('.diagram-element')));
        if (element.hasClass('diagram-element-s') ||
            element.hasClass('diagram-element-v') ||
            element.hasClass('diagram-element-vp') ||
            element.hasClass('diagram-element-do') ||
            element.hasClass('diagram-element-pn') ||
            element.hasClass('diagram-element-pa'))
        {
            height = Math.max(height, element.getWordHeight());
        }
        else if (element.hasClass('diagram-element-ip') ||
                 element.hasClass('diagram-element-np') ||
                 element.hasClass('diagram-element-ger'))
        {
            //console.log("maxTopHeight: pedestal");
            height = Math.max(height, element.getPedestalHeight() + getTopHeight(element.children('.diagram-clause')));
        }
        else if (element.hasClass('diagram-element-conj'))
        {
            height = Math.max(height, getTopHeight(element.children('.digram-clause')));
        }
    });
    return height;
}
function getTopHeight(elements)
{
    var height = 0;
    elements.each(function() {
        var element = $(this);
        //console.log("getTopHeight: element="+element.attr('class'));
        height += maxTopHeight(element.children('.diagram-element'));
    });
    return height;
}
jQuery.fn.getWordHeight = function()
{
    var line = this.children('.diagram-part-line, .diagram-part-curved');
    if (line.length)
    {
        var word = line.children('.diagram-word');
        if (word.length)
        {
            var span = word.children('span');
            if (span.length)
            {
                return span.height() + 5;
            }
        }
    }
    return 20;
}

function getWidthWithModifiers(elements, includeAppositives)
{
    var total = 0;
    elements.each(function() {
        var element = $(this);
        if (!includeAppositives && element.hasClass('diagram-element-app'))
        {
            return true; // continue elements.each
        }
        var children = element.children();
        if (children.length)
        {
            total += getWidthWithModifiers(children, true);
        }
        if (element.hasClass('diagram-part-line-width-skip'))
        {
            return true; // continue elements.each
        }
        if (element.hasClass('diagram-part-line'))
        {
            total += element.getWidth();
        }
        if (element.hasClass('diagram-part-curved'))
        {
            var p = transformRotate(
                    {x: Math.max(element.parent().getWordWidth(), 50), y: 0},
                    'rotate(45deg)',
                    {x: 0, y: 0}
                );
            total += p.x;
        }
        if (element.hasClass('diagram-part-line-width-break'))
        {
            var appositive = element.next('.diagram-element-app');
            if (appositive.length)
            {
                total += appositive.children('.diagram-part-line').getWidth();
            }
            return false; // break elements.each
        }
    });
    return total;
}
function getWidthWithAppositives(elements)
{
    var total = 0;
    elements.each(function() {
        var element = $(this);
        var children = element.children();
        children.each(function() {
            var child = $(this);
            if (child.hasClass('diagram-part-line-width-skip'))
            {
                return true; // continue elements.each
            }
            if (child.hasClass('diagram-part-line'))
            {
                total += child.getWidth();
            }
            if (child.hasClass('diagram-part-curved'))
            {
                var p = transformRotate(
                        {x: Math.max(child.parent().getWordWidth(), 50), y: 0},
                        'rotate(45deg)',
                        {x: 0, y: 0}
                    );
                total += p.x;
            }
            if (child.hasClass('diagram-element-app'))
            {
                total += child.children('.diagram-part-line').getWidth();
            }
        });
    });
    return total;
}
jQuery.fn.getWidth = function()
{
    elementOffset = this.offset();
    p1 = { x: elementOffset.left, y: elementOffset.top };
    p2 = { x: elementOffset.left + this.width(), y: p1.y };
    var transform = this.css('transform');
    if (transform && transform.search('none') < 0)
    {
        p2 = transformRotate(p2, transform, p1);
    }
    return p2.x - p1.x;
}
jQuery.fn.getWordWidth = function()
{
    if (this.hasClass('diagram-word'))
    {
        return this.getWidthOfChildren('span') + 20 * 2;
    }
    else if (this.hasClass('diagram-part-line') || this.hasClass('diagram-part-curved') || this.hasClass('diagram-part-dashed'))
    {
        return this.children('.diagram-word').first().getWordWidth();
    }
    else if (this.hasClass('diagram-element'))
    {
        return this.children('.diagram-part-line, .diagram-part-curved, .diagram-part-dashed').first().getWordWidth();
    }
    return 50;
}
jQuery.fn.getWidthOfChildren = function(selector)
{
    var total = 0;
    this.children(selector).each(function() {
        total += $(this).width();
    });
    return total;
}

jQuery.fn.getPedestalHeight = function()
{
    if (this.hasClass('diagram-element-ip') ||
        this.hasClass('diagram-element-np'))
    {
        var pedestalLeft = this.children('.diagram-part-pedestal-left');
        if (!pedestalLeft.length) alert("cannot find diagram-part-pedestal-left");
        var pedestalLeftTransform = pedestalLeft.css('transform');
        var pedestalLeftTop = transformRotate({x: pedestalLeft.width(), y: 0}, pedestalLeftTransform, {x: 0, y: 0});
        var pedestalCenter = this.children('.diagram-part-pedestal-center');
        if (!pedestalCenter.length) alert("cannot find diagram-part-pedestal-center");
        return -pedestalLeftTop.y + pedestalCenter.width();
    }
    else if (this.hasClass('diagram-element-ger'))
    {
        var pedestal = this.children('.diagram-part-pedestal');
        if (!pedestal.length) alert("cannot find diagram-part-pedestal");
        return pedestal.width();
    }
    alert("cannot get pedestal height");
}
jQuery.fn.getPedestalWidth = function()
{
    if (this.hasClass('diagram-element-ip') ||
        this.hasClass('diagram-element-np'))
    {
        var pedestalLeft = this.children('.diagram-part-pedestal-left');
        if (!pedestalLeft.length) alert("cannot find diagram-part-pedestal-left");
        return pedestalLeft.getWidth() * 2;
    }
    else if (this.hasClass('diagram-element-ger'))
    {
        return 0;
    }
    alert("cannot get pedestal width");
}
jQuery.fn.getPedestalClauseOffset = function()
{
    return {x: this.getPedestalWidth() / 2, y: -this.getPedestalHeight()};
}

jQuery.fn.setWidth = function()
{
    if (this.hasClass('diagram-word')) {
        this.width(this.getWidthOfChildren('span') + 20 * 2);
    }
    else if (this.hasClass('diagram-clause')) {
        // do nothing
    }
    else if (this.hasClass('diagram-element-s') ||
        this.hasClass('diagram-element-v') ||
        this.hasClass('diagram-element-do') ||
        this.hasClass('diagram-element-pn') ||
        this.hasClass('diagram-element-pa') ||
        this.hasClass('diagram-element-adj') ||
        this.hasClass('diagram-element-adv') ||
        this.hasClass('diagram-element-art') ||
        this.hasClass('diagram-element-pos') ||
        this.hasClass('diagram-element-app')) {
        this.width(Math.max(this.getWordWidth(), getWidthWithModifiers(this.children('.diagram-part'), false) + /*space*/ 20));
    }
    else if (this.hasClass('diagram-element-io-o') ||
        this.hasClass('diagram-element-prep-o')) {
        this.width(Math.max(this.getWordWidth(), getWidthWithModifiers(this.children('.diagram-part'), true) + /*space*/ 20));
    }
    else if (this.hasClass('diagram-element-io') ||
        this.hasClass('diagram-element-prep')) {
        this.width(this.getWordWidth());
    }
    else if (this.hasClass('diagram-element-part')) {
        this.width(this.getWordWidth());
    }
    else if (this.hasClass('diagram-element-ip') ||
        this.hasClass('diagram-element-np')) {
        // pedestal center width (height) is the max height of all modifiers
        var pedestalCenter = this.children('.diagram-part-pedestal-center');
        if (!pedestalCenter.length) alert("cannot find diagram-part-pedestal-center");
        pedestalCenter.width(Math.max(40, maxModifiersHeight(this.children('.diagram-clause').children('.diagram-part'))));
    }
    else if (this.hasClass('diagram-element-ip-v-d')) {
        this.width(this.getWordWidth());
    }
    else if (this.hasClass('diagram-element-ip-v-h')) {
        this.width(Math.max(this.getWordWidth(), getWidthWithModifiers(this.children('.diagram-part'), false) + /*space*/ 20));
    }
    else if (this.hasClass('diagram-element-ger')) {
        // pedestal width (height) is gerund-phrase height
        var pedestal = this.children('.diagram-part-pedestal');
        if (!pedestal.length) alert("cannot find diagram-part-pedestal");
        pedestal.width(Math.max(40, maxModifiersHeight(this.children('.gerund-phrase').children('.diagram-part')) + 20));
    }
    else if (this.hasClass('gerund-phrase')) {
        // do nothing - elements have widths
    }
    else if (this.hasClass('diagram-element-ger-v')) {
        // staircase width(s) based on word width - ger & prep-ger
        var staircaseTop = this.children('.diagram-part-staircase-top');
        if (!staircaseTop.length) alert("cannot find diagram-part-staircase-top");
        var staircaseMiddle = this.children('.diagram-part-staircase-middle');
        if (!staircaseMiddle.length) alert("cannot find diagram-part-staircase-middle");
        var staircaseBottom = this.children('.diagram-part-staircase-bottom');
        if (!staircaseBottom.length) alert("cannot find diagram-part-staircase-bottom");
        var wordWidth = this.getWordWidth();
        var p = transformRotate({ x: wordWidth, y: 0 }, 'rotate(15deg)', { x: 0, y: 0 });
        staircaseTop.width(p.x * (1 / 3));
        staircaseMiddle.width(p.y);
        staircaseBottom.width(p.x * (2 / 3));
    }
    else if (this.hasClass('diagram-element-prep-ger')) {
        this.width(this.getWordWidth());
    }
    else if (this.hasClass('diagram-element-exp')) {
        this.width(this.getWordWidth());
    }
    else if (this.hasClass('diagram-element-conj-s') ||
        this.hasClass('diagram-element-conj-v') ||
        this.hasClass('diagram-element-conj-vp') ||
        this.hasClass('diagram-element-conj-do') ||
        this.hasClass('diagram-element-conj-pn') ||
        this.hasClass('diagram-element-conj-pa')) {
        // will set width in setOffset with drawLine
    }
    else if (this.hasClass('diagram-element-conj-adj') ||
             this.hasClass('diagram-element-conj-adv') ||
             this.hasClass('diagram-element-conj-art') ||
             this.hasClass('diagram-element-conj-pos'))
    {
        var dashed = this.children('.diagram-part-dashed');
        if (!dashed.length) alert("cannot find diagram-part-dashed");
        dashed.width(dashed.getWordWidth());
    }

    // TODO - conj, conj-v, etc

    return this;
}
jQuery.fn.setOffset = function()
{
    if (this.hasClass('diagram-word'))
    {
        this.css({'top': 0 - this.height()});
    }
    else if (this.hasClass('diagram-clause'))
    {
        if (this.parent().hasClass('diagram-element-ip') ||
            this.parent().hasClass('diagram-element-np'))
        {
            var clauseOffset = this.parent().getPedestalClauseOffset();
            this.css({'left': clauseOffset.x, 'top': clauseOffset.y});
        }
    }
    else if (this.hasClass('diagram-element-s'))
    {
        // leave offset as 0,0
    }
    else if (this.hasClass('diagram-element-v') ||
                this.hasClass('diagram-element-do') ||
                this.hasClass('diagram-element-pn') ||
                this.hasClass('diagram-element-pa'))
    {
        if (this.parent().hasClass('diagram-element-part'))
        {
            var p = transformRotate({x: this.parent().getWordWidth(), y:0}, 'rotate(45deg)', {x: 0, y: 0});
            this.css({'left': 0 + p.x, 'top': 0 + p.y - 3});
        }
        else if (this.parent().hasClass('diagram-element-ip-v-h'))
        {
            this.css({'left': this.parent().getWidth()});
        }
        else
        {
            this.css({'left': getWidthWithAppositives(this.prevAll('.diagram-part'))});
        }
    }
    else if (this.hasClass('diagram-element-adj') ||
                this.hasClass('diagram-element-adv') ||
                this.hasClass('diagram-element-art') ||
                this.hasClass('diagram-element-pos') ||
                this.hasClass('diagram-element-io') ||
                this.hasClass('diagram-element-prep'))
    {
        this.css({'left': getWidthWithModifiers(this.prevAll('.diagram-part'), false) + /*space*/ 20});
    }
    else if (this.hasClass('diagram-element-io-o') ||
                this.hasClass('diagram-element-prep-o'))
    {
        var diagonal = this.prev('.diagram-part-diagonal');
        var diagonalOffset = diagonal.offset();
        var transform = diagonal.css('transform');
        var diagonalBottom = transformRotate({x:diagonalOffset.left + diagonal.width(), y:diagonalOffset.top + diagonal.height()}, transform, {x: diagonalOffset.left, y:diagonalOffset.top});
        this.css({'left': diagonalBottom.x - diagonalOffset.left, 'top': diagonalBottom.y - diagonalOffset.top - 3});
    }
    else if (this.hasClass('diagram-element-app'))
    {
        this.css({'left': this.parent().width()});
    }
    else if (this.hasClass('diagram-element-part'))
    {
        this.css({'left': getWidthWithModifiers(this.prevAll('.diagram-part'), false) + /*space*/ 20});
        var p = transformRotate({x: this.getWordWidth(), y:0}, 'rotate(45deg)', {x: 0, y: 0});
        var canvas = this.children('.diagram-part-curved').children('canvas')[0];
        if (canvas.getContext)
        {
            var context = canvas.getContext('2d');
            context.clearRect(0, 0, canvas.width, canvas.height);
            canvas.width = Math.max(Math.ceil(p.x/100)*100, 300);
            canvas.height = Math.max(Math.ceil(p.x/100)*100, 150);
            context.beginPath();
            context.moveTo(0, 0);
            context.arcTo(0, p.y, p.x, p.y, p.x);
            context.stroke();
            var word = this.children('.diagram-part-curved').children('.diagram-word');
            if (word.length)
            {
                word.css({
                        'top': 0 - canvas.height - word.height(),
                        'transform': 'rotate(45deg) translate('+word.height()/2+'px, 0px)', 
                        'transform-origin': '0 0'
                    });
            }
        }
        else
        {
            alert("can draw a participle... your browswer doesn't support canvas");
            return;
        }
    }
    else if (this.hasClass('diagram-element-ip'))
    {
        // get width of diagonal verb
        var pedestalVerbDiagonal = this.children('.diagram-clause').children('.diagram-element-ip-v-d').children('.diagram-part-line');
        if (!pedestalVerbDiagonal) alert("cannot find ip-v-d line");
        var verbWidth = pedestalVerbDiagonal.getWidth();
        // left line offset
        var pedestalLeft = this.children('.diagram-part-pedestal-left');
        if (!pedestalLeft.length) alert("cannot find diagram-part-pedestal-left");
        pedestalLeft.css({'left': verbWidth});
        var leftWidth = pedestalLeft.getWidth();
        // right line offset
        var pedestalRight = this.children('.diagram-part-pedestal-right');
        if (!pedestalRight.length) alert("cannot find diagram-part-pedestal-right");
        pedestalRight.css({'left': verbWidth + leftWidth * 2, 'top': 3});
        // center line offset
        var pedestalCenter = this.children('.diagram-part-pedestal-center');
        if (!pedestalCenter.length) alert("cannot find diagram-part-pedestal-center");
        pedestalCenter.css({'left': verbWidth + leftWidth - 1, 'top': 0 - leftWidth + 3});
    }
    else if (this.hasClass('diagram-element-ip-v-d'))
    {
        // get width of diagonal verb
        var pedestalVerbDiagonal = this.children('.diagram-part-line');
        if (!pedestalVerbDiagonal) alert("cannot find ip-v-d line");
        var bottom = transformRotate({x: pedestalVerbDiagonal.width(), y: 0}, 'rotate(60deg)', {x: 0, y: 0});
        this.css({'left': 2, 'top': -bottom.y + 2});
    }
    else if (this.hasClass('diagram-element-ip-v-h'))
    {
        this.css({'left': getWidthWithAppositives(this.prevAll('.diagram-part'))});
    }
    else if (this.hasClass('diagram-element-np'))
    {
        // get width of subject
        var pedestalSubject = this.children('.diagram-clause').children('.diagram-element-s');
        if (!pedestalSubject) alert("cannot find np s");
        var subjectWidth = pedestalSubject.getWordWidth();
        // get width of verb
        var pedestalVerb = this.children('.diagram-clause').children('.diagram-element-v, .diagram-element-vp');
        if (!pedestalVerb) alert("cannot find np v");
        var verbWidth = pedestalVerb.getWordWidth();
        // left line offset
        var pedestalLeft = this.children('.diagram-part-pedestal-left');
        if (!pedestalLeft.length) alert("cannot find diagram-part-pedestal-left");
        pedestalLeft.css({'left': subjectWidth + (verbWidth / 2)});
        var leftWidth = pedestalLeft.getWidth();
        // right line offset
        var pedestalRight = this.children('.diagram-part-pedestal-right');
        if (!pedestalRight.length) alert("cannot find diagram-part-pedestal-right");
        pedestalRight.css({'left': subjectWidth + (verbWidth / 2) + (leftWidth * 2), 'top': 3});
        // center line offset
        var pedestalCenter = this.children('.diagram-part-pedestal-center');
        if (!pedestalCenter.length) alert("cannot find diagram-part-pedestal-center");
        pedestalCenter.css({'left': subjectWidth + (verbWidth / 2) + leftWidth - 1, 'top': 0 - leftWidth + 3});
    }
    else if (this.hasClass('diagram-element-ger'))
    {
        // pedestal offset
        var gerundVerb = this.children('.gerund-phrase').children('.diagram-element-ger-v');
        if (!gerundVerb.length) alert("cannot find ger-v");
        var verbWidth = gerundVerb.getWordWidth();
        var pedestal = this.children('.diagram-part-pedestal');
        if (!pedestal.length) alert("cannot find diagram-part-pedestal");
        pedestal.css({'left': verbWidth / 2, 'top': 3});
    }
    else if (this.hasClass('gerund-phrase'))
    {
        if (this.parent().hasClass('diagram-element-ger'))
        {
            // set gerund-phrase offsets based on pedestal
            var gerundOffset = this.parent().getPedestalClauseOffset();
            this.css({'left': gerundOffset.x, 'top': gerundOffset.y});
        }
        else if (this.parent().hasClass('diagram-element-prep-ger'))
        {
            // bottom of preposition diagonal
            var diagonal = this.prev('.diagram-part-diagonal');
            var diagonalOffset = diagonal.offset();
            var transform = diagonal.css('transform');
            var diagonalBottom = transformRotate({x:diagonalOffset.left + diagonal.width(), y:diagonalOffset.top + diagonal.height()}, transform, {x: diagonalOffset.left, y:diagonalOffset.top});
            // height of staircase
            var wordWidth = this.children('.diagram-element-ger-v').getWordWidth();
            var p = transformRotate({x: wordWidth, y: 0}, 'rotate(15deg)', {x: 0, y: 0});
            this.css({'left': (diagonalBottom.x - diagonalOffset.left), 'top': (diagonalBottom.y - diagonalOffset.top - 3 + p.y)});
        }
    }
    else if (this.hasClass('diagram-element-ger-v'))
    {
        // staircase offsets - ger & prep-ger
        var staircaseTop = this.children('.diagram-part-staircase-top');
        if (!staircaseTop.length) alert("cannot find diagram-part-staircase-top");
        var staircaseMiddle = this.children('.diagram-part-staircase-middle');
        if (!staircaseMiddle.length) alert("cannot find diagram-part-staircase-middle");
        var staircaseBottom = this.children('.diagram-part-staircase-bottom');
        if (!staircaseBottom.length) alert("cannot find diagram-part-staircase-bottom");
        var wordWidth = this.getWordWidth();
        var p = transformRotate({x: wordWidth, y: 0}, 'rotate(15deg)', {x: 0, y: 0});
        staircaseTop.css({'top': -p.y});
        staircaseMiddle.css({'left': p.x * (1/3), 'top': -p.y});
        staircaseBottom.css({'left': p.x * (1/3) - 3});
        // diagram-word offsets and transform
        var word = staircaseBottom.children('.diagram-word');
        if (word.length)
        {
            word.css({
                    'transform': 'rotate(15deg) translate('+(-word.width()*(1/3))+'px, '+(-p.y)+'px)',
                    'transform-origin': '0 0'
                });
        }
    }
    else if (this.hasClass('diagram-element-prep-ger'))
    {
        this.css({'left': getWidthWithModifiers(this.prevAll('.diagram-part'), false) + /*space*/ 20});
    }
    else if (this.hasClass('diagram-element-exp'))
    {
        // verb width
        var phrase = this.parent();
        var phraseOffset = phrase.offset();
        var verb = phrase.children('.diagram-element-v, .diagram-element-vp');
        if (!verb.length) alert("cannot find diagram-element-v/vp")
        var verbWidth = verb.getWidth();
        var verbOffset = verb.offset();
        // get dashed line
        var dashed = this.children('.diagram-part-dashed');
        if (!dashed.length) alert("cannot find diagram-part-dashed");
        // word width
        var wordWidth = this.getWordWidth();
        // offset of exp
        this.css({'left': verbOffset.left - phraseOffset.left + (verbWidth / 2) - (wordWidth / 2), 'top': -20 + -dashed.width()});
        // offset of dashed line
        dashed.css({'left': (wordWidth / 2)});
    }
    else if(this.hasClass('diagram-element-conj-s') ||
            this.hasClass('diagram-element-conj-v') ||
            this.hasClass('diagram-element-conj-vp') ||
            this.hasClass('diagram-element-conj-do') ||
            this.hasClass('diagram-element-conj-pn') ||
            this.hasClass('diagram-element-conj-pa'))
    {
        // offset of horizontal
        var horizontal = this.children('.diagram-part-horizontal');
        if (!horizontal.length) alert("cannot find diagram-part-horizontal");
        horizontal.css({'left': getWidthWithAppositives(this.prevAll('.diagram-part'))});
        // get dashed, splits and clauses
        var dashed = this.children('.diagram-part-dashed');
        if (!dashed.length) alert("cannot find diagram-part-dashed");
        var splits = this.children('.diagram-part-split');
        if (!splits.length) alert("cannot find diagram-part-split");
        var clauses = this.children('.diagram-clause');
        if (!clauses.length) alert("cannot find diagram-clause");
        var height = getBottomHeight(clauses) + getTopHeight(clauses) - getTopHeight(clauses.first());
        dashed.width(Math.max(height, dashed.getWordWidth()));
        height = Math.max(height, dashed.width());
        var top = -height + getTopHeight(clauses.first());
        var left = getWidthWithAppositives(this.prevAll('.diagram-part')) + horizontal.width() + 20;
        // draw dashed line
        dashed.css({'left': left + 1, 'top': top});
        // top
        clauses.first().css({'left': left, 'top': top});
        splits.first().drawLine(left - 20, 0, left, top);
        // bottom
        clauses.last().css({'left': left, top: top + height});
        splits.last().drawLine(left - 20, 0, left, top + height)
        // middle (if any)
        for (var i = 1; i < clauses.length - 1 && i < splits.length - 1; i++)
        {
            var split = $(splits[i]);
            var clause = $(clauses[i]);
            top += getTopHeight(clause);
            split.drawLine(left - 20 + i, 0, left + i, top);
            clause.css({'left': left + i, 'top': top});
            top += getBottomHeight(clause);
        }
    }
    else if (this.hasClass('diagram-element-conj-adj') ||
        this.hasClass('diagram-element-conj-adv') ||
        this.hasClass('diagram-element-conj-art') ||
        this.hasClass('diagram-element-conj-pos')) {
        var dashed = this.children('.diagram-part-dashed');
        if (!dashed.length) alert("cannot find diagram-part-dashed");
        var elements = this.children('.diagram-element');
        if (!elements.length) alert("cannot find diagram-element");
        var height = dashed.getWordHeight();
        var offset = getWidthWithModifiers(this.prevAll('.diagram-part'), false) + /*space*/ 20;
        var p = transformRotate({ x: offset + height, y: 0 }, elements.first().children('.diagram-part-line').css('transform'), { x: offset, y: 0 });
        dashed.css({ 'left': p.x, 'top': p.y });
        elements.first().css({ 'left': offset });
        elements.last().css({ 'left': offset + dashed.width() });
        // TODO - more than two elements
    }

    // TODO - conj, conj-v, etc

    return this;
}
jQuery.fn.setWidths = function()
{
    var children = this.children();
    if (children.length)
    {
        children.each(function() { $(this).setWidths() });
    }
    return this.setWidth();
}
jQuery.fn.setOffsets = function()
{
    var children = this.children();
    if (children.length)
    {
        children.each(function() { $(this).setOffsets() });
    }
    return this.setOffset();
}
jQuery.fn.drawClause = function()
{
    // TODO - find top clause(es) and draw all children
    var clause = this.closest('.diagram-clause');
    if (!clause.length)
    {
        return this;
    }
    // all widths must be calculated before any offsets can be calculated
    clause.children().each(function() { $(this).setWidths(); });
    clause.children().each(function() { $(this).setOffsets(); });
    return clause.parent().drawClause();
}

function sortParts(a, b)
{
    if ($(a).text() === 'conj') return -1;
    if ($(b).text() === 'conj') return 1;
    if ($(a).text() === 'prep' && $(b).text() === 'ger') return -1;
    if ($(a).text() === 'ger' && $(b).text() === 'prep') return 1;
    return 0;
}
function getPartsText(parts)
{
    parts = parts.sort(sortParts);
    var part = '';
    parts.each(function() {
        var element = $(this);
        if (part != '') part += '-';
        part += element.text();
    });
    return part;
}

jQuery.fn.addElement = function(part, counter)
{
    return $('<div />')
        .css({'min-width': 50})
        .drawPart(part, counter)
        .addClass('diagram-part')
        .addClass('diagram-element')
        .addClass('diagram-element-' + part)
        .attr('id', 'diagram-element-' + counter)
        .addClass('diagram-element-' + counter)
        .appendTo(this)
        .updateCounter(counter);
}
jQuery.fn.drawPart = function(part, counter)
{
    switch(part)
    {
        case 's':
            // on hortzontal before vertical bisect
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-s-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            break;
        case 'v':
            // on horizontal after vertical bisect
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-'+part+'-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(90deg)'})
                .width(25 + 3)
                .offset({left: 0, top: -20})
                .attr('id', 'diagram-part-'+part+'-vertical')
                .addClass('diagram-part-vertical')
                .addClass('diagram-part-line-height-skip')
                .appendTo(this);
            break;
        case 'do':
            // on horizontal after vertical stop
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-do-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .addClass('diagram-part-line-width-break')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(90deg)'})
                .width(20 + 3)
                .offset({left: 0, top: -20})
                .attr('id', 'diagram-part-do-vertical')
                .addClass('diagram-part-vertical')
                .addClass('diagram-part-line-height-skip')
                .appendTo(this);
            break;
        case 'pn':
        case 'pa':
            // on horizontal after slash (\)
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-do-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(60deg)'})
                .width(20 + 3)
                .offset({left: 0, top: -20})
                .attr('id', 'diagram-part-do-vertical')
                .addClass('diagram-part-vertical')
                .addClass('diagram-part-line-height-skip')
                .appendTo(this);
            break;
        case 'part':
            // curved word on bent, slanted below word
            var participle = $('<div />')
                .attr('id', 'diagram-part-part-curved')
                .addClass('diagram-part-curved')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            $('<canvas />')
                .css({'all': 'revert'})
                .appendTo(participle);
            break;
        case 'adj':
        case 'adv':
        case 'art':
        case 'pos':
            // on diagonal below word
            $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(60deg)'})
                .width('100%')
                .attr('id', 'diagram-part-'+part+'-diagonal')
                .addClass('diagram-part-diagonal')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            break;
        case 'io':
        case 'prep':
            // indirect object on horizontal at bottom of diagonal below word
            // preposition on diagonal below word, object on horizontal at bottom of diagonal
            var diagonal = $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(60deg)'})
                .width('100%')
                .attr('id', 'diagram-part-'+part+'-diagonal')
                .addClass('diagram-part-diagonal')
                .appendTo(this);
            if (part === 'prep')
            {
                diagonal.addClass('diagram-part-word-target');
            }
            this.addElement('prep-o', counter);
            break;
        case 'io-o':
        case 'prep-o':
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-'+part+'-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .addClass('diagram-part-line-width-break')
                .appendTo(this);
            break;
        case 'app':
            // in parentheses to the right of word
            var horizontal = $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-s-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            horizontal.addWord($('<span>(</span><span>)</span>'), counter);
            break;
        case 'ip':
            // on pedestal beginning with diagonal
            $('<div />')
                .addClass('diagram-part-line')
                .width(20)
                .css({'transform': 'rotate(-45deg)'})
                .attr('id', 'diagram-part-ip-pedestal-left')
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-pedestal-left')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .width(20)
                .css({'transform': 'rotate(-135deg)'})
                .attr('id', 'diagram-part-ip-pedestal-right')
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-pedestal-right')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .width(40)
                .css({'transform': 'rotate(-90deg)'})
                .attr('id', 'diagram-part-ip-pedestal-center')
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-pedestal-center')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            var clause = $('<div />')
                .addClass('diagram-clause')
                .appendTo(this);
            clause.addElement('ip-v-d', counter);
            clause.addElement('ip-v-h', counter);
            break;
        case 'ip-v-d':
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .css({'transform': 'rotate(60deg)'})
                .attr('id', 'diagram-part-ip-v-diagonal')
                .addClass('diagram-part-ip-v-diagonal')
                .addClass('diagram-part-word-target')
                .addClass('diagram-part-line-height-skip')
                .appendTo(this);
            break;
        case 'ip-v-h':
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-ip-v-horizontal')
                .addClass('diagram-part-ip-v-horizontal')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .addClass('diagram-part-line-width-break')
                .appendTo(this);
            break;
        case 'np':
            // on pedestal with horizontal (recurse) and broken line from explective to verb
            $('<div />')
                .addClass('diagram-part-line')
                .width(20)
                .css({'transform': 'rotate(-45deg)'})
                .attr('id', 'diagram-part-np-pedestal-left')
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-pedestal-left')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .width(20)
                .css({'transform': 'rotate(-135deg)'})
                .attr('id', 'diagram-part-np-pedestal-right')
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-pedestal-right')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .width(40)
                .css({'transform': 'rotate(-90deg)'})
                .attr('id', 'diagram-part-np-pedestal-center')
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-pedestal-center')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            var clause = $('<div />')
                .addClass('diagram-clause')
                .appendTo(this);
            clause.addElement('s', counter);
            clause.addElement('v', counter);
            break;
        case 'ger':
            // on pedestal beginning with staircase
            $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(-90deg)'})
                .attr('id', 'diagram-part-ger-pedestal')                        
                .addClass('diagram-part-pedestal')
                .addClass('diagram-part-line-height-skip')
                .appendTo(this);
            var gerund = $('<div />')
                .addClass('gerund-phrase')
                .appendTo(this);
            gerund.addElement('ger-v', counter);
            // gerund does not require a do, so don't automatically add one
            break;
        case 'ger-v':
            // staircase
            $('<div />')
                .addClass('diagram-part-line')
                .attr('id', 'diagram-part-ger-v-staircase-horizontal-top')
                .addClass('diagram-part-staircase-top')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(90deg)'})
                .attr('id', 'diagram-part-ger-v-staircase-vertical')
                .addClass('diagram-part-staircase-middle')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .attr('id', 'diagram-part-ger-v-staircase-horizontal-bottom')
                .addClass('diagram-part-staircase-bottom')
                .addClass('diagram-part-target')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            break;
        case 'prep-ger':
            // on staircase on diagonal of preposition (object of preposition)
            var diagonal = $('<div />')
                .addClass('diagram-part-line')
                .css({'transform': 'rotate(60deg)'})
                .width('100%')
                .attr('id', 'diagram-part-'+part+'-diagonal')
                .addClass('diagram-part-diagonal')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            var gerund = $('<div />')
                .addClass('gerund-phrase')
                .appendTo(this);
            gerund.addElement('ger-v', counter);
            break;
        case 'exp':
            // on horizontal connected to modified word via broken - introduces a noun phrase
            $('<div />')
                .addClass('diagram-part-dashed-line')
                .css({'transform': 'rotate(90deg)'})
                .width(36) /* set width - this will be the only thing above a verb */
                .attr('id', 'diagram-part-exp-dashed-vertical')
                .addClass('diagram-part-dashed')
                .addClass('diagram-part-vertical')
                .addClass('diagram-part-line-width-break')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            $('<div />')
                .addClass('diagram-part-line')
                .width('100%')
                .attr('id', 'diagram-part-exp-horizontal')
                .addClass('diagram-part-horizontal')
                .addClass('diagram-part-word-target')
                .addClass('diagram-part-line-width-skip')
                .appendTo(this);
            break;
        case 'conj':
            // on broken line between words
            // TODO
            alert("cannot draw conj - not implemented");
            break;
        case 'conj-s':
        case 'conj-v':
        case 'conj-do':
        case 'conj-pn':
        case 'conj-pa':
            this.addClass('diagram-element-conj');
            $('<div />')
                .addClass('diagram-part-line')
                .width(20)
                .attr('id', 'diagram-part-conj-horizontal')
                .addClass('diagram-part-horizontal')
                .appendTo(this);
            var split1 = $('<div />')
                .addClass('diagram-part-line')
                .attr('id', 'diagram-part-split-1')
                .addClass('diagram-part-split')
                .appendTo(this);
            var clause1 = $('<div />')
                .addClass('diagram-clause')
                .appendTo(this);
            clause1.addElement(part.substring(5), counter);
            var split2 = $('<div />')
                .addClass('diagram-part-line')
                .attr('id', 'diagram-part-split-2')
                .addClass('diagram-part-split')
                .appendTo(this);
            var clause2 = $('<div />')
                .addClass('diagram-clause')
                .appendTo(this);
            clause2.addElement(part.substring(5), counter);
            $('<div />')
                .addClass('diagram-part-dashed-line')
                .css({'transform': 'rotate(90deg)'})
                .attr('id', 'diagram-part-conj-dashed-vertical')
                .addClass('diagram-part-dashed')
                .addClass('diagram-part-vertical')
                .addClass('diagram-part-line-width-break')
                .addClass('diagram-part-line-width-skip')
                .addClass('diagram-part-word-target')
                .appendTo(this);        
            break;
        case 'conj-io':
        case 'conj-prep':
            // TODO
            alert("can't draw conj-io/prep - not implemented");
            break;
        case 'conj-part':
            // TODO
            alert("can't draw conj-part - not implemetned");
            break;
        case 'conj-adj':
        case 'conj-adv':
        case 'conj-art':
        case 'conj-pos':
            this.addClass('diagram-element-conj');
            this.addElement(part.substring(5), counter);
            this.addElement(part.substring(5), counter);
            $('<div />')
                .addClass('diagram-part-dashed-line')
                .attr('id', 'diagram-part-conj-dashed-horizontal')
                .addClass('diagram-part-dashed')
                .addClass('diagram-part-line')
                .addClass('diagram-part-word-target')
                .appendTo(this);
            break;
        case 'conj-app':
        case 'conj-ip':
        case 'conj-ger':
        case 'conj-exp':
        case 'conj-np':
            // TODO
            //break;
            alert("cannot draw a "+part+", not implemented");
            return;
        default:
            alert("something is very wrong.. don't know how to draw a "+part);
            return;
    }
    return this;
}
jQuery.fn.addPart = function(parts, event)
{
    var container = this;
    var part = getPartsText(parts);
    var counter = diagramElementCounter + 1;
    switch (part)
    {
        case 's':
        case 'conj-s':
            // a subject requires a new clause - TODO - upgrade s to conj-s
            container = $('#diagram');
            container = $('<div />')
                .addClass('diagram-clause')
                .offset({left: event.originalEvent.offsetX, top: event.originalEvent.offsetY})
                .appendTo(container);
            break;
        case 'v':
        case 'conj-v':
        case 'do':
        case 'conj-do':
        case 'pn':
        case 'conj-pn':
        case 'pa':
        case 'conj-pa':
            // a verb, verb phrase, direct object, predicate nominative, or predicate adjective
            // applies to an existing clause, participle, or gerund phrase
            container = container.closest('.diagram-element-part, .gerund-phrase, .diagram-clause').first();
            if (!container.length)
            {
                alert("cannot add "+part+", clause required")
                return;
            }
            switch (part)
            {
                case 'v':
                    // there can only be a single verb or verb phrase - TODO - upgrade v to conj-v
                    if (container.children('.diagram-element-v').length)
                    {
                        alert("cannot add a second "+part+" to the clause");
                        return;
                    }
                    // a verb or verb phrase requires an existing subject - TODO - or conj-s
                    if (!container.children('.diagram-element-s').length)
                    {
                        alert("cannot add a "+part+", subject required");
                        return;
                    }
                    break;
                case 'do':
                case 'pn':
                case 'pa':
                    // there can only be a single direct object, predicate nominative, or predicate adjective
                    // TODO - upgrade to conj-do, conj-pa, conj-pa
                    if (container.children('.diagram-element-do, .diagram-element-pn, .diagram-element-pa').length)
                    {
                        alert("cannot add a second "+part+" to the clause");
                        return;
                    }
                    // a direct object, predicate nominative, or predicate adjective
                    // requires an existing verb or verb phrase
                    if (!container.children('.diagram-element-v').length &&
                        !container.hasClass('diagram-element-part') &&
                        !container.children('.diagram-element-ip-v-h').length &&
                        !container.children('.diagram-element-ger-v').length)
                    {
                        alert("cannot add a "+part+", verb/verb-phrase required");
                        return;
                    }
                    break;
            }
            break;
        case 'io':
        // TODO - case 'conj-io':
            // an indirect object requries a verb or verb phrase
            container = container.closest('.diagram-element-v, .diagram-element-vp').first();
            if (!container.length)
            {
                alert("cannot add an indirect object, verb/verb-phrase required");
                return;
            }
            break;
        case 'exp':
        // TODO - case 'conj-exp':
            // an expletive requires a noun phrase
            container = container.closest('.diagram-element-np').first().children('.diagram-clause');
            if (!container.length)
            {
                alert("cannot add an explective, noun phrase required");
                return;
            }
            break;
        case 'conj':
            // a conjunction applies to two or more existing parts
            // TODO - upgrade container to conj-<container-part> OR if container is conj-<part> add another <part> to conj-<part>
            alert("cannot add a conjunction, not implemented");
            return;
            break;
        case 'app':
        // TODO - case 'conj-app':
            // TODO - more than one appositive
            //break;
        case 'part':
        // TODO - case 'conj-part':
        case 'adj':
        case 'conj-adj':
        case 'adv':
        case 'conj-adv':
        case 'art':
        case 'conj-art':
        case 'pos':
        case 'conj-pos':
        case 'prep':
        // TODO - case 'conj-prep':
            // TODO - if container is prep, then upgrade prep to prep-ger
        case 'ip':
        // TODO - case 'conj-ip':
        case 'ger':
        // TODO - case 'conj-ger':
        case 'prep-ger':
        // TODO - case 'conj-prep-ger':
        case 'np':
        // TODO - case 'conj-np':
            // an indirect object, participal, adjective, adverb, article, possessive, preposition, appositive, infinitive phrase, gerund, explective, or noun phrase
            // requires an existing part
            // TODO - conjunctions
            container = container.closest('.diagram-part').not('.diagram-element-part');
            if (!container.length)
            {
                alert("cannot add a "+part+", element required")
                return;
            }
            break;
        default:
            alert("something is very wrong... don't know what to do with a " + part);
            return;
    }
    //console.log("addPart: part="+part+" container=" + container.attr('id'));
    return container
        .addElement(part, counter)
        .drawClause();
}
var diagramElementCounter = 0;
//var margin = 20;
jQuery.fn.addWord = function(words, counter)
{
    //console.log("addWord: this="+this.attr('id')+" "+this.attr('class')+", word="+words.text()+", word.width="+words.width()+", word.height="+words.height());
    var div = $('<div />')
        .attr('id', 'diagram-element-' + counter)
        .addClass('diagram-word')
        .addClass('diagram-element')
        .addClass('diagram-element-' + counter)
        .css({'text-align': 'center'})
        .appendTo(this);
    words.each(function() {
        $('<span />')
            .text($(this).text())
            .appendTo(div);
    })
    return this;
}
jQuery.fn.updateWord = function(words, counter)
{
    //console.log("updateWord: this="+this.attr('id')+" "+this.attr('class')+", words="+words.text());

    /*if (this.parent().parent().hasClass('.diagram-element-v'))
    {
        // TODO - upgrade v to vp
    }
    else*/ if (this.parent().parent().hasClass('diagram-element-app'))
    {
        var elements = this.children('span');
        if (!elements.length)
        {
            alert("something is very wrong... appositive does not have ()");
            return;
        }
        else if (elements.length === 2)
        {
            elements.last().before($('<span />').text(words.text()));
        }
        else if (elements.length === 3)
        {
            $(elements[1]).text(words.text());
        }
    }
    // TODO - expletives
    // TODO - are there other special cases?
    else
    {
        this.addClass('diagram-element-'+counter)
            .children('span')
                .text(words.text());
    }
    return this;
}
jQuery.fn.addOrUpdateWord = function(words)
{
    //console.log("addOrUpdateWord: this="+this.attr('id')+" "+this.attr('class')+", words="+words.text());
    var counter = diagramElementCounter + 1;
    var diagramWord = this.children('.diagram-word');
    if (diagramWord.length)
    {
        diagramWord.updateWord(words, counter);
    }
    else
    {
        this.addWord(words, counter);
    }
    return this
        .updateCounter(counter)
        .drawClause();
}
function getEventTarget(event) {
    event = event || window.event;
    return event.target || event.srcElement;
}

$(document).ready(function() {
    var sentence = "Supercalifragalistic is simply quite atrocious.";
    var words = sentence.split(/[ ,]+/);
    words.forEach(function(word) {
        $('<span />').appendTo('#sentence')
            .addClass('sentence-word')
            .attr('id', word)
            .text(word);
        $('<span />').appendTo('#sentence')
            .addClass('sentence-space')
            .text(" ");
    });
    $('#sentence .sentence-word')
    .draggable({
            cancel: '.no-drag',
            revert: true,
            helper: 'clone',
            opacity: 0.3
    })
    .click(function(event) {
        $('#sentence .sentence-word').removeClass('active');
        $(this).addClass('active');
    });

    var parts = [
        ['subject', 's'],
        ['verb', 'v'],
        ['direct object', 'do'],
        ['indirect object', 'io'],
        ['predicate nominative', 'pn'],
        ['predicate adjective', 'pa'],
        ['participle', 'part'],
        ['adjective', 'adj'],
        ['adverb', 'adv'],
        ['article', 'art'],
        ['possessive', 'pos'],
        ['preposition', 'prep'],
        ['conjunction', 'conj'],
        ['appositive', 'app'],
        ['infinitive phrase', 'ip'],
        ['gerund', 'ger'],
        ['expletive', 'exp'],
        ['noun phrase', 'np'] ];
    parts.forEach(function(part) {
        $('<button />').appendTo('#parts')
            .attr('id', 'part-button-'+part[1])
            .addClass('part-button')
            .addClass('part-button-'+part[1])
            .text(part[1]);
        $('<span />').appendTo('#parts')
            .text(' ');
    });
    $('#parts button').click(function(event) {
        var element = $(this);
        if (element.hasClass('active'))
        {
            element.removeClass('active');
            return;
        }
        if (element.hasClass('part-button-ger'))
        {
            $('#parts button').not('.part-button-conj').not('.part-button-prep').removeClass('active');
        }
        else
        {
            $('#parts button').not('.part-button-conj').removeClass('active');
        }
        element.addClass('active');
    })
    .draggable({
            cancel: '.no-drag',
            revert: true,
            helper: 'clone',
            opacity: 0.3
    });

    $('#diagram').click(function(event) {
        $(this).focus();
        var activeButtons = $('#parts .active');
        if (activeButtons.length)
        {
            var target = $('#diagram .diagram-part-target').closestToOffset({left: event.originalEvent.pageX, top: event.originalEvent.pageY});
            if (target)
            {
                //console.log("#diagram click : " + target.attr('id') + " -> " + activeButtons.text());
                target.addPart(activeButtons, event);
            }
            else
            {
                //console.log("#diagram click : " + this.id + " -> " + activeButtons.text());
                $(this).addPart(activeButtons, event);
            }
            activeButtons.removeClass('active');
            return;
        }
        var activeText = $('#sentence .active');
        if (activeText.length)
        {
            var target = $('#diagram .diagram-part-word-target').closestToOffset({left: event.originalEvent.pageX, top: event.originalEvent.pageY});
            if (target)
            {
                //console.log("#diagram click : " + target.attr('id') + " -> " + activeText.text());
                target.addOrUpdateWord(activeText);
                activeText.removeClass('active');
                return;
            }
        }
        var targetWord = $("#diagram .diagram-word").closestToOffset({left: event.originalEvent.pageX, top: event.originalEvent.pageY});
        if (targetWord)
        {
            //console.log("#diagram click : word=" + targetWord.text());
            targetWord.addClass('active');
        }
    })
    .droppable({
        drop: function(event, ui) {
            $(this).focus();
            var draggable = $(ui.draggable);
            if (draggable.hasClass('part-button'))
            {
                // TODO - conj-s, conj-v, etc
                var target = $('#diagram .diagram-part-target').closestToOffset({left: event.originalEvent.pageX, top: event.originalEvent.pageY}); 
                if (target)
                {
                    //console.log("#diagram drop : " + target.attr('id') + " -> " + draggable.text());
                    target.addPart(draggable, event);
                }
                else
                {
                    //console.log("#diagram drop : " + this.id + " -> " + draggable.text());
                    $(this).addPart(draggable, event);
                }
                $(draggable).removeClass('active');
                return;
            }
            if (draggable.hasClass('sentence-word'))
            {
                var target = $('#diagram .diagram-part-word-target').closestToOffset({left: event.originalEvent.pageX, top: event.originalEvent.pageY});
                if (target)
                {
                    //console.log("#diagram drop : " + target.attr('id') + " -> " + draggable.text());
                    target.addOrUpdateWord($(draggable));
                    $(draggable).removeClass('active');
                }
                return;
            }
        }
    })
    /*.keydown(function(event) {
        event.preventDefault();
        console.log("#diagram keydown : keyCode="+event.keyCode+", which="+event.which+", ctrlKey="+event.ctrlKey);
    })*/
    .keyup(function(event) {
        event.preventDefault();
        console.log("#diagram keyup : keyCode="+event.keyCode+", which="+event.which+", ctrlKey="+event.ctrlKey);
        switch (event.keyCode)
        {
            case 46:
                //console.log("#diagram keyup: delete");
                var selectedWord = $("#diagram .diagram-word.active");
                if (selectedWord.length)
                {
                    //console.log("#diagram keyup: remove " + selectedWord.text());
                    selectedWord.remove();
                }
                break;
            //case 17:
            //    console.log("#diagram keyup: ctrl");
            //    break;
            case 90:
                //console.log("#diagram keyup: z");
                if (event.ctrlKey)
                {
                    //console.log("#digram keyup: ctrl-z");
                    lastElement = $('.diagram-element-' + diagramElementCounter);
                    if (lastElement.length)
                    {
                        //console.log("#diagram keyup: remove " + lastElement.attr('id'));
                        lastElement.remove();
                        --diagramElementCounter;
                    }
                }
                break;
        }
    });
});